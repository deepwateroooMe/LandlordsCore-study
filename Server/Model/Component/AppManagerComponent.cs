using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
namespace ETModel {

    [ObjectSystem]
    public class AppManagerComponentAwakeSystem : AwakeSystem<AppManagerComponent> {
        public override void Awake(AppManagerComponent self) {
            self.Awake();
        }
    }

    public class AppManagerComponent: Component {
        private readonly Dictionary<int, Process> processes = new Dictionary<int, Process>();

        public void Awake() {
            string[] ips = NetHelper.GetAddressIPs();
            StartConfig[] startConfigs = StartConfigComponent.Instance.GetAll();
            
            foreach (StartConfig startConfig in startConfigs) {
                Game.Scene.GetComponent<TimerComponent>().WaitAsync(100); // 间隔0.1秒
                
                if (!ips.Contains(startConfig.ServerIP) && startConfig.ServerIP != "*") {
                    continue;
                }
                if (startConfig.AppType.Is(AppType.Manager)) {
                    continue;
                }
                StartProcess(startConfig.AppId); // 启动开启进程
            }
            this.WatchProcessAsync(); // 监控进程：不要让它挂掉了，如安卓应用般，挂掉了被系统杀死了，就再把它拉活
        }
        private void StartProcess(int appId) {
            OptionComponent optionComponent = Game.Scene.GetComponent<OptionComponent>();
            StartConfigComponent startConfigComponent = StartConfigComponent.Instance;
            string configFile = optionComponent.Options.Config;
            StartConfig startConfig = startConfigComponent.Get(appId);
            const string exe = "dotnet";
            string arguments = $"App.dll --appId={startConfig.AppId} --appType={startConfig.AppType} --config={configFile}";
            Log.Info($"{exe} {arguments}");
            try {
                bool useShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                ProcessStartInfo info = new ProcessStartInfo { FileName = exe, Arguments = arguments, CreateNoWindow = true, UseShellExecute = useShellExecute }; // 启动一个进程所需要的参数
                Process process = Process.Start(info); // 启动进程，拿到进程的实例
                this.processes.Add(startConfig.AppId, process); // 放字典里，管理
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        // 监控启动的进程,如果进程挂掉了,重新拉起. 就是对服务器端的宕机，保活
        private async void WatchProcessAsync() {
            long instanceId = this.InstanceId;
            
            while (true) {
                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(5000);
                if (this.InstanceId != instanceId) {
                    return;
                }
                foreach (int appId in this.processes.Keys.ToArray()) {
                    Process process = this.processes[appId];
                    if (!process.HasExited)
                    {
                        continue;
                    }
                    this.processes.Remove(appId);
                    process.Dispose();
                    this.StartProcess(appId);
                }
            }
        }
    }
}