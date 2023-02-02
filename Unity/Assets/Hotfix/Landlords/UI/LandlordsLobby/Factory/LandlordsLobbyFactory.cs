using ETModel;
using System;
using UnityEngine;
namespace ETHotfix {

    [UIFactory(UIType.LandlordsLobby)]
    public class LandlordsLobbyFactory : IUIFactory {
// type: LandLordsLogin LandlordsLobby LandlordsRoom etc 表明的是UI预设的字符串名字
        public UI Create(Scene scene, string type, GameObject parent) { 
            try {
                // 加载AB包
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d"); // 每个预设打成了独立的资源包，显得粒度偏细偏小,但它维护了可装载可卸载的最小组件粒度需求
                // 加载大厅界面预设并生成实例
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject lobby = UnityEngine.Object.Instantiate(bundleGameObject);
                // 设置UI层级，只有UI摄像机可以渲染
                lobby.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(lobby);
                ui.AddComponent<LandlordsLobbyComponent>();
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
