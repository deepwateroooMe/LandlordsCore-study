using ETModel;
using System;
using UnityEngine;
namespace ETHotfix {
    // 它说，要工厂化生产各种UI 组件，这里工厂部门专职负责生产 UIType.LandlordsLobby 这个类型的UI 场景
    [UIFactory(UIType.LandlordsLobby)]
    public class LandlordsLobbyFactory : IUIFactory {

        public UI Create(Scene scene, string type, GameObject parent) {
            try {
                // 加载AB包
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>(); // 这个组件，负责各种热更新资源包的加载管理等
                resourcesComponent.LoadBundle($"{type}.unity3d"); // 想要加载的热更新资源包的名字叫： landlordslobby.unity3d 项目里所有资源包后缀为 .unity3d
                // 加载大厅界面预设并生成实例
// 从landlordslobby.unity3d 热更新资源包中加载 LandlordsLobby 预设，注意大小写字母名字严格匹配，否则出错。【预设】名字自调用处传进来，来自于LandlordsLoginComponent.cs 里的调用
// Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsLobby); // 去找：这个UI 组件的创建，与相关按钮回调过程
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}"); // 拿到热更新资源包中想要加载的预设（模板） 

                GameObject lobby = UnityEngine.Object.Instantiate(bundleGameObject); // 照着预设生成一个当前所需要的实例
                // 设置UI层级，只有UI摄像机可以渲染
                lobby.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(lobby); // 创建：自动化调用了 LandlordsLobbyComponent.cs 的 Awake() 方法初始化
                ui.AddComponent<LandlordsLobbyComponent>(); // 这里，才是真正产生父子级关系的地方，在UI 组件上添加了 LandlordsLobbyComponent 组件
                return ui;
            }
            catch (Exception e) {
                Log.Error(e.ToStr());
                return null;
            }
        }
        public void Remove(string type) {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}
