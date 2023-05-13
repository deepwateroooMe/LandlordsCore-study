using System;
namespace ETHotfix {
    public static class ComponentFactory {

 // CreateWithParent() 一  二  三  个参数        
        public static Component CreateWithParent(Type type, Component parent) {
            Component component = Game.ObjectPool.Fetch(type);
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }
        public static T CreateWithParent<T>(Component parent) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }
        public static T CreateWithParent<T, A>(Component parent, A a) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a);
            return component;
        }
        public static T CreateWithParent<T, A, B>(Component parent, A a, B b) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b);
            return component;
        }
        public static T CreateWithParent<T, A, B, C>(Component parent, A a, B b, C c) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            component.Parent = parent;
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }

// Create() 一  二  三  个参数        
        public static T Create<T>() where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component);
            return component;
        }
        public static T Create<T, A>(A a) where T : Component { // 这里有一点儿没看懂：父级关系是如何设置出来的？
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a); // 这里是加载组件时，自动调用组件的Awake()生命周期回调函数
            return component;
        }
        public static T Create<T, A, B>(A a, B b) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b);
            return component;
        }
        public static T Create<T, A, B, C>(A a, B b, C c) where T : Component {
            T component = Game.ObjectPool.Fetch<T>();
            ComponentWithId componentWithId = component as ComponentWithId;
            if (componentWithId != null) {
                componentWithId.Id = component.InstanceId;
            }
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }
 //  CreateWithId() 一  二  三  个参数等
        public static T CreateWithId<T>(long id) where T : ComponentWithId {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component);
            return component;
        }
        public static T CreateWithId<T, A>(long id, A a) where T : ComponentWithId {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a);
            return component;
        }
        public static T CreateWithId<T, A, B>(long id, A a, B b) where T : ComponentWithId {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a, b);
            return component;
        }
        public static T CreateWithId<T, A, B, C>(long id, A a, B b, C c) where T : ComponentWithId {
            T component = Game.ObjectPool.Fetch<T>();
            component.Id = id;
            Game.EventSystem.Awake(component, a, b, c);
            return component;
        }
    }
}
