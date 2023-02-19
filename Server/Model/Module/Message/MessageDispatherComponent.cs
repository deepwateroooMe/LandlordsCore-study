using System.Collections.Generic;
namespace ETModel {
    // 消息分发组件
    public class MessageDispatherComponent : Component {
        public readonly Dictionary<ushort, List<IMHandler>> Handlers = new Dictionary<ushort, List<IMHandler>>();
    }
}