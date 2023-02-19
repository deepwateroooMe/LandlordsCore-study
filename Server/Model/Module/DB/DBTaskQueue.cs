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
    // 这个，没太看懂
    public sealed class DBTaskQueue : Component { // 关于异步任务的部分，数据库任务缓存壳加工厂：壳是对数据库异步任务的包装，感觉像是数据库异步任务的对象池
        public Queue<DBTask> queue = new Queue<DBTask>(); // 这里不曾说任务是单一的，可以混杂的，可是查询，增删查改等混杂的
        public TaskCompletionSource<DBTask> tcs;          // 还是看得狠昏：有个空的可以执行并写结果的壳？

        public void Add(DBTask task) { // 这里加的是什么意思，居然没有看明白
            if (this.tcs != null) {
                var t = this.tcs;
                this.tcs = null;   // 置空了
                t.SetResult(task); // 直接执行当前任务 ?
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