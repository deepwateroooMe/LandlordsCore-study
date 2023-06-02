using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETModel {

    public struct ActorMessageInfo {
        public Session Session;
        public object Message;
    }

    // 挂上这个组件表示该Entity是一个Actor,接收的消息将会队列处理( 可以把 actor 理解为邮件工人，可以收发邮件。同一个系统工人之间是可以收发邮件的 )
    public class MailBoxComponent: Component {

        // 拦截器类型，默认没有拦截器
        public string ActorInterceptType;
        // 队列处理消息
        public Queue<ActorMessageInfo> Queue = new Queue<ActorMessageInfo>();
        public TaskCompletionSource<ActorMessageInfo> Tcs; // 这个组件，总是带唯一一个当前任务的

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            var t = this.Tcs;
            this.Tcs = null; // 置空
            t?.SetResult(new ActorMessageInfo()); // 设置结果
        }
    }
}
