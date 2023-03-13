using System;
using System.IO;
using System.Threading;
using Google.Protobuf;
using UnityEngine;

namespace ETModel {
    // TODO: 这里，需要去比较一下， et7 里与 et4 的斗地主里的起始有什么区别，去理解 7 里起始时作者标的那句话
    // 客户端入口程序： 
    public class Init : MonoBehaviour {
        private async void Start() {
            try { 
                if (!Application.unityVersion.StartsWith("2017.4")) {
                    Log.Error($"新人请使用Unity2017.4版本,减少跑demo遇到的问题! 下载地址:\n https:// unity3d.com/cn/unity/qa/lts-releases?_ga=2.227583646.282345691.1536717255-1119432033.1499739574");
                }
                // 这里，这个，就是主线程了呀
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance); // 异步线程等的上下文自动同步
                DontDestroyOnLoad(gameObject);
                
// Unity.Model 里面的代码不能热更新，通常将游戏中不会变动的部分放在这个项目里。这个Game 应该是用来双端共享的
                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly); 
                Game.Scene.AddComponent<GlobalConfigComponent>(); // 读取全局配置，不知道是否会触发什么回调
                Game.Scene.AddComponent<NetOuterComponent>();     // ETModel 里，客户端需要与登录服，网关服通消息，必须挂这个，后面客户端起始的时候就用到这个
                Game.Scene.AddComponent<ResourcesComponent>();
                Game.Scene.AddComponent<PlayerComponent>();
                Game.Scene.AddComponent<UnitComponent>();
                Game.Scene.AddComponent<UIComponent>();
                // 斗地主客户端自定义全局组件
                // 用于保存玩家本地数据
                Game.Scene.AddComponent<ClientComponent>(); // ETModel 启动的时候会添加这个客户端组件 
                // 下载ab包
                await BundleHelper.DownloadBundle(); // <<<<<<<<<<<<<<<<<<<< 这里调用：  下载热更新资源包，准备热更新
                Game.Hotfix.LoadHotfixAssembly();    // 加载热更新程序集
                // 加载配置
                Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                Game.Scene.AddComponent<ConfigComponent>();
                Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
                Game.Scene.AddComponent<OpcodeTypeComponent>(); // 发消息的时候就会用到这个
                Game.Scene.AddComponent<MessageDispatherComponent>(); // 消息分发器
                Game.Hotfix.GotoHotfix(); // 这里负责：热更新程序域的初始化配置，相关适配加载等
// EventIdType: 这个类里，定义了程序域相关一些事件
                Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribeMonoEvent");
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