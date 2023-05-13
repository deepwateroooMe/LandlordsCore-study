using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using UnityEngine;
namespace ETHotfix {
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
    // 管理所有UI:
    public class UIComponent: Component {
        private GameObject Root;
        private readonly Dictionary<string, IUIFactory> UiTypes = new Dictionary<string, IUIFactory>();
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
            this.UiTypes.Clear();
            this.uis.Clear();
        }
        public void Awake() {
            this.Root = GameObject.Find("Global/UI/");
            this.Load();
        }
// 加载系统：注意这个UI 控件系统，与其它系统标签不同的是：
        // 其它系统标签比如消息处理器标注，会在加载的时候自动创建生成了一个消息处理器的实例。
        // 这里UI 控件系统，并不生成相应于UI 类型的单个实例，而是生成制造生产这种UI 类型的UI 分类型控件加工厂
        public void Load() { 
            UiTypes.Clear();
            
            List<Type> types = Game.EventSystem.GetTypes();
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof (UIFactoryAttribute), false); // 它扫描的标签是UI 控件分类型的加工厂，但凡是UI 加工厂，就生产一个实例时刻准备着。。。
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
                UI ui = UiTypes[type].Create(this.GetParent<Scene>(), type, Root);
                uis.Add(type, ui);
                // 设置canvas
                string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;
                ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);
                return ui;
            }
// 暂时不管下面的这个：第三个玩家从Unity 中运行的情况，我原本只是想要引擎中去查看下，点击后房间生成的控件
            catch (Exception e) { // 这里我有点儿没有弄明白：当我的第三个玩家是从 Unity 中运行的时候，为什么会抛异常？是什么资源没能准备好？LandlordsRoom
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
            UiTypes[type].Remove(type);
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