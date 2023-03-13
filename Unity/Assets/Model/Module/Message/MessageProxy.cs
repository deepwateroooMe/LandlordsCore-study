using System;
namespace ETModel {
    public class MessageProxy: IMHandler { // 消息代理

        private readonly Type type;
        private readonly Action<Session, object> action; // 消息回调

        public MessageProxy(Type type, Action<Session, object> action) { // 消息回调方法：是以参数传入进来的
            this.type = type;
            this.action = action; // 消息回调方法：是以参数传入进来的
        }
        
        public void Handle(Session session, object message) {
            this.action.Invoke(session, message);
        }
        public Type GetMessageType() {
            return this.type;
        }
    }
}
