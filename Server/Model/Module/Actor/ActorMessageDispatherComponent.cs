using System;
using System.Collections.Generic;
namespace ETModel {

    // Actor消息分发组件：这个模块还没吃透
    public class ActorMessageDispatherComponent : Component {

        // 为什么要定义丙个字典来着？
        public readonly Dictionary<string, IActorInterceptTypeHandler> ActorTypeHandlers = new Dictionary<string, IActorInterceptTypeHandler>(); // 分配给消息处理器？
        public readonly Dictionary<Type, IMActorHandler> ActorMessageHandlers = new Dictionary<Type, IMActorHandler>(); // 一类消息的分发：是中转，还是下发？

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.ActorMessageHandlers.Clear();
            this.ActorTypeHandlers.Clear();
        }
    }
}