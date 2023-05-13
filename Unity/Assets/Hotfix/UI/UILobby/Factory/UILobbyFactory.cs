using System;
using ETModel;
using UnityEngine;
namespace ETHotfix {
    [UIFactory(UIType.UILobby)]
    public class UILobbyFactory : IUIFactory {
        public UI Create(Scene scene, string type, GameObject gameObject) {
            try {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject lobby = UnityEngine.Object.Instantiate(bundleGameObject);
                lobby.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(lobby); // 这里会自动调用触发 Awake()
                // 记得上次这里作过笔记：这个添加的过程，就是添加子控件，为什么我的笔记都不见了呢？
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