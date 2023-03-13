using System;
using System.Collections.Generic;
namespace ETModel {
    // 前面说过，知道对方的地址，就可以给对方发消息
    // 那么这个组件的意思是说：作为MessageDispatcher 的一个组件，帮助统一管理不同会话框的许多发送端 ActorLocationSender 信息
    public class ActorLocationSenderComponent: Component {
        // 本地缓存 
        public readonly Dictionary<long, ActorLocationSender> ActorLocationSenders = new Dictionary<long, ActorLocationSender>();

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose(); // 这里昏起来：不是一般先从继承类清理，再清理基类吗？这里，树梢上的若是不清理完毕，树根能清理掉吗？
            // 值是引用类型，先清理引用类型的值，再清理键 
            foreach (ActorLocationSender actorLocationSender in this.ActorLocationSenders.Values) {
                actorLocationSender.Dispose();
            }
            this.ActorLocationSenders.Clear();
        }

        public ActorLocationSender Get(long id) {
            if (id == 0) {
                throw new Exception($"actor id is 0");
            }
            if (this.ActorLocationSenders.TryGetValue(id, out ActorLocationSender actorLocationSender)) {
                return actorLocationSender; // 直接返回缓存
            } // 缓存没有，就创建新的，并缓存起来
            actorLocationSender = ComponentFactory.CreateWithId<ActorLocationSender>(id);
            actorLocationSender.Parent = this;
            this.ActorLocationSenders[id] = actorLocationSender;
            return actorLocationSender;
        }

        public void Remove(long id) {
            if (!this.ActorLocationSenders.TryGetValue(id, out ActorLocationSender actorMessageSender)) {
                return;
            }
            this.ActorLocationSenders.Remove(id);
            actorMessageSender.Dispose();
        }
    }
}
