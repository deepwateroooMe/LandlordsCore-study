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
 // 只要开始，它就繁忙地执行所有待处理，分配给它的任务，像勤劳的小蜜蜂一样。。。。。
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

 // 这里应该是说：服务器端自带缓存机制，应该也可以理解为，它有个专用线程什么之类的，作为后备军劳力，只要有任务，这个线程就去运行，以便在最快的时间之内能够返回给客户端？
 // 上面是初步猜测的，要用源码来验证是否猜测正确？？？
    public sealed class DBTaskQueue : Component { // 关于异步任务的部分，

        public Queue<DBTask> queue = new Queue<DBTask>(); // 这里不曾说任务是单一的，可以混杂的，可是查询，增删查改等混杂的
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