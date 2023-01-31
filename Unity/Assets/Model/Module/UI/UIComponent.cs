using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ETModel {

    [ObjectSystem]
    public class UiComponentAwakeSystem : AwakeSystem<UIComponent> {
        public override void Awake(UIComponent self) {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class UiComponentLoadSystem : LoadSystem<UIComponent> {
        public override void Load(UIComponent self) {
            self.Load();
        }
    }

// 管理所有UI
    public class UIComponent: Component {
        private GameObject Root;

        private readonly Dictionary<string, IUIFactory> UiTypes = new Dictionary<string, IUIFactory>(); // 不同类型,有不同的实例实现创建与移除的逻辑,不同的实现类
        private readonly Dictionary<string, UI> uis = new Dictionary<string, UI>();

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            foreach (string type in uis.Keys.ToArray()) {
                UI ui;
                if (!uis.TryGetValue(type, out ui)) {
                    continue;
                }
                uis.Remove(type);
                ui.Dispose();
            }
            this.uis.Clear();
            this.UiTypes.Clear();
        }

        public void Awake() {
            this.Root = GameObject.Find("Global/UI/"); // 这里应该是查找的游戏视图视图里的某个元件,对,这里作为所有UI可装卸组件的树根
            this.Load();
        }
        public void Load() { // 加载好,各种实例化UI画工厂
            this.UiTypes.Clear();
            
            List<Type> types = Game.EventSystem.GetTypes(typeof(UIFactoryAttribute)); // 系统里有UI工厂标签
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof (UIFactoryAttribute), false);
                if (attrs.Length == 0) {
                    continue;
                }
                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (UiTypes.ContainsKey(attribute.Type)) {
                    Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
                    throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
                }
                object o = Activator.CreateInstance(type);
                IUIFactory factory = o as IUIFactory;
                if (factory == null) {
                    Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                this.UiTypes.Add(attribute.Type, factory);
            }
        }

        public UI Create(string type) {
            try {
                UI ui = UiTypes[type].Create(this.GetParent<Scene>(), type, Root); // 要求其相应画工厂生产出一个当前场景下,当前根节点下的某个类型的UI控件
                uis.Add(type, ui);
                // 设置canvas: 就是当前控件与父画布的关系，设置好 
                string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;
                ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);
                return ui;
            }
            catch (Exception e) {
                throw new Exception($"{type} UI 错误: {e}");
            }
        }
        public void Add(string type, UI ui) {
            this.uis.Add(type, ui);
        }
        public void Remove(string type) {
            UI ui;
            if (!uis.TryGetValue(type, out ui)) {
                return;
            }
            uis.Remove(type);
            ui.Dispose();
        }
        public void RemoveAll() {
            foreach (string type in this.uis.Keys.ToArray()) {
                UI ui;
                if (!this.uis.TryGetValue(type, out ui)) {
                    continue;
                }
                this.uis.Remove(type);
                ui.Dispose();
            }
        }
        public UI Get(string type) {
            UI ui;
            this.uis.TryGetValue(type, out ui);
            return ui;
        }
        public List<string> GetUITypeList() {
            return new List<string>(this.uis.Keys);
        }
    }
}