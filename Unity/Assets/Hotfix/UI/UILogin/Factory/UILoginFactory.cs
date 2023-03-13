using System;
using ETModel;
using UnityEngine;
namespace ETHotfix {

    [UIFactory(UIType.UILogin)] // 对不同的 UI 类型，都有一个对应的加工厂：来负责这种类型的生产加工出厂
    public class UILoginFactory : IUIFactory {

        public UI Create(Scene scene, string type, GameObject gameObject) {
            try {
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                login.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(login);
                ui.AddComponent<UILoginComponent>();
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

