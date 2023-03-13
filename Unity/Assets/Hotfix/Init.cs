using System;
using ETModel;

namespace ETHotfix {
    public static class Init {

        public static void Start() { // <<<<<<<<<<<<<<<<<<<< 从 ETModel.Init 里指定说，运行热更新程序域里的 Start() 方法 
            try {
#if ILRuntime
                if (!Define.IsILRuntime) {
                    Log.Error("mono层是mono模式, 但是Hotfix层是ILRuntime模式");
                }
#else
                if (Define.IsILRuntime) {
                    Log.Error("mono层是ILRuntime模式, Hotfix层是mono模式");
                }
#endif
                Game.Scene.ModelScene = ETModel.Game.Scene; // 客户端妄动的时候，会有实例化一个客户端的，模型场景 
                // 注册热更层回调
                ETModel.Game.Hotfix.Update = () => { Update(); };
                ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
                ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };
                
                Game.Scene.AddComponent<UIComponent>();
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                Game.Scene.AddComponent<MessageDispatherComponent>();
                // 加载热更配置
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                Game.Scene.AddComponent<ConfigComponent>();
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
                UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
                Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");
                
                // Game.EventSystem.Run(EventIdType.InitSceneStart);
// 客户端说：要进到这个起始场景中来，会触发调用这个LandlordsInitSceneStart 类型Event 标签的类的Run() 方法
                Game.EventSystem.Run(EventIdType.LandlordsInitSceneStart); 
            }
            catch (Exception e) { 
                Log.Error(e);
            }
        }
        public static void Update() {
            try {
                Game.EventSystem.Update();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        public static void LateUpdate() {
            try {
                Game.EventSystem.LateUpdate();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        public static void OnApplicationQuit() {
            Game.Close();
        }
    }
}
