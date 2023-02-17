using System;
using ETModel;
using UnityEngine;
namespace ETHotfix {
    [UIFactory(UIType.UILobby)]

    // 游戏大厅：存放的是当前这里（游戏开发者？）所有的游戏，我可能也想要开发5 － 10 个个人游戏吧，放大厅里。再点击进入游戏后，根据所选择的游戏加载热重载资源包
    // 大厅是登录后，还是登录前？是登录后，登录的用户可以玩当前开发者开放的几乎所有的游戏
    
    public class UILobbyFactory : IUIFactory {

        public UI Create(Scene scene, string type, GameObject gameObject) {
            try {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");  // 这里加载，相关资源包的名字
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject lobby = UnityEngine.Object.Instantiate(bundleGameObject);
                lobby.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(lobby);
                ui.AddComponent<UILobbyComponent>();
                return ui;
            }
            catch (Exception e) {
                Log.Error(e);
                return null;
            }
        }
        public void Remove(string type) {
            ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}