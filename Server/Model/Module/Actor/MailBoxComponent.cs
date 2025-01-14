﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETModel {

    public struct ActorMessageInfo {
        public Session Session;
        public object Message;
    }

    // 挂上这个组件表示该Entity是一个Actor,接收的消息将会队列处理(意思是说，它可以内网服务器意发送消息？)
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
            this.Tcs = null; // 置空，当前 ActorMessageInfo 才可以回收
            t?.SetResult(new ActorMessageInfo()); // 把当前最新最后任务的结果返回回去
        }
    }
}