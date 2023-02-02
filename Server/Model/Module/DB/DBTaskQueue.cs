using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETModel {

    [ObjectSystem]
    public class DbTaskQueueAwakeSystem : AwakeSystem<DBTaskQueue> {
        public override void Awake(DBTaskQueue self) {
            self.queue.Clear();
        }
    }
    [ObjectSystem]
    public class DbTaskQueueStartSystem : StartSystem<DBTaskQueue> {
        public override async void Start(DBTaskQueue self) {
            long instanceId = self.InstanceId;
            
            while (true) {
                if (self.InstanceId != instanceId) {
                    return;
                }
                DBTask task = await self.Get();
                try {
                    await task.Run();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
                task.Dispose();
            }
        }
    }

    public sealed class DBTaskQueue : Component { // 关于异步任务的部分，看来这里还是有点儿没有弄明白

        public Queue<DBTask> queue = new Queue<DBTask>();
        public TaskCompletionSource<DBTask> tcs;

        public void Add(DBTask task) { // 这里加的是什么意思，居然没有看明白
            if (this.tcs != null) {
                var t = this.tcs;
                this.tcs = null; // 置空了
                t.SetResult(task); // 先前的任务这里写结果吗？
                return;
            }
            this.queue.Enqueue(task);
        }

        public Task<DBTask> Get() {
            if (this.queue.Count > 0) {
                DBTask task = this.queue.Dequeue();
                return Task.FromResult(task);
            }
            TaskCompletionSource<DBTask> t = new TaskCompletionSource<DBTask>();
            this.tcs = t;
            return t.Task;
        }
    }
}