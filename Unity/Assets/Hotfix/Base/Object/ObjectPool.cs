using System;
using System.Collections.Generic;
namespace ETHotfix {
    public class ObjectPool { // 各种类的对象池
        // 尽可能地缓存有实例，供拿去用
        private readonly Dictionary<Type, Queue<Component>> dictionary = new Dictionary<Type, Queue<Component>>();

        public Component Fetch(Type type) {
            Queue<Component> queue;
            if (!this.dictionary.TryGetValue(type, out queue)) {
                queue = new Queue<Component>();
                this.dictionary.Add(type, queue);
            }
            Component obj;
            if (queue.Count > 0) {
                obj = queue.Dequeue(); // 池子里有，就拿个去用
            }
            else {
                obj = (Component)Activator.CreateInstance(type); // 没有才创建一个新的实例
            }
            obj.IsFromPool = true;
            return obj;
        }
        public T Fetch<T>() where T: Component {
            T t = (T) this.Fetch(typeof(T));
            return t;
        }
        
        public void Recycle(Component obj) {
            Type type = obj.GetType();
            Queue<Component> queue;
            if (!this.dictionary.TryGetValue(type, out queue)) {
                queue = new Queue<Component>();
                this.dictionary.Add(type, queue);
            }
            queue.Enqueue(obj);
        }
    }
}