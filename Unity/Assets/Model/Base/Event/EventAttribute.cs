using System;

namespace ETModel {

    // 它说，这是事件类型标签属性。那么事件是如何触发的呢？只要程序逻辑里触发或是调用了某标签系标明的事件，这些所有标签标明了这个同一Event 类型的类里的方法回调，都会一一调用？
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class EventAttribute: BaseAttribute { 

        public string Type { get; }
        public EventAttribute(string type) { // 【Event(Type)】标签
            this.Type = type;
        }
    }
}