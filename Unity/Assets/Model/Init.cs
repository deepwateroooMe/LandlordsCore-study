using System;
using System.IO;
using System.Threading;
using Google.Protobuf;
using UnityEngine;

namespace ETModel {
    public class Init : MonoBehaviour {

        private async void Start() {
            try { 
                if (!Application.unityVersion.StartsWith("2017.4")) {
                    Log.Error($"新人请使用Unity2017.4版本,减少跑demo遇到的问题! 下载地址:\n https:// unity3d.com/cn/unity/qa/lts-releases?_ga=2.227583646.282345691.1536717255-1119432033.1499739574");
                }
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance); // 异步线程等的上下文自动同步
                DontDestroyOnLoad(gameObject);
                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly); // Unity.Model 里面的代码不能热更新，通常将游戏中不会变动的部分放在这个项目里
                Game.Scene.AddComponent<GlobalConfigComponent>(); // 读取全局配置，不知道是否会触发什么回调
                Game.Scene.AddComponent<NetOuterComponent>(); // 客户端需要与登录服，网关服通消息，必须挂这个
                Game.Scene.AddComponent<ResourcesComponent>();
                Game.Scene.AddComponent<PlayerComponent>();
                Game.Scene.AddComponent<UnitComponent>();
                Game.Scene.AddComponent<UIComponent>();
                // 斗地主客户端自定义全局组件
                // 用于保存玩家本地数据
                Game.Scene.AddComponent<ClientComponent>();
                // 下载ab包
                await BundleHelper.DownloadBundle();
                Game.Hotfix.LoadHotfixAssembly();
                // 加载配置
                Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                Game.Scene.AddComponent<ConfigComponent>();
                Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                Game.Scene.AddComponent<MessageDispatherComponent>();
                Game.Hotfix.GotoHotfix();
                Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void Update() {
            OneThreadSynchronizationContext.Instance.Update();
            Game.Hotfix.Update?.Invoke();
            Game.EventSystem.Update();
        }
        private void LateUpdate() {
            Game.Hotfix.LateUpdate?.Invoke();
            Game.EventSystem.LateUpdate();
        }

        private void OnApplicationQuit() {
            Game.Hotfix.OnApplicationQuit?.Invoke();
            Game.Close();
        }
    }
}