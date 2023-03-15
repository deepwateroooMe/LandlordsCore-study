using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ETModel {

    // 昨天客户端的Loom 类：不同再次真切地体验了一回？
    // 当网络异步线程的结果不能同步到主线程时，可能会发生些什么吗？【日志明确显示服务器逻辑处理成功，给客户端分配了网关服，返回了其地址等，但客户端主线和接收不到，因为不曾同步到主线程中去！！！】
    public class OneThreadSynchronizationContext : SynchronizationContext {

        public static OneThreadSynchronizationContext Instance { get; } = new OneThreadSynchronizationContext();
        private readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        private Action a;

        public void Update() {
            while (true) {
                if (!this.queue.TryDequeue(out a)) {
                    return;
                }
                a();
            }
        }

        public override void Post(SendOrPostCallback callback, object state) {
            // 如果是主线程Post则直接执行回调，不需要进入队列
            if (Thread.CurrentThread.ManagedThreadId == this.mainThreadId) {
                callback(state);
                return;
            }
            this.queue.Enqueue(() => { callback(state); });
        }
    }
}
