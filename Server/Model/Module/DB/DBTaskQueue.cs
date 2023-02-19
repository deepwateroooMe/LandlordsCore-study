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
            while (true) { // 无限循环
                if (self.InstanceId != instanceId) { // 过程中：可能存在什么问题？
                    return;
                }
// 需要获取一个数据库任务 DBTask. 如果缓存队列里没有任务就会处于等待状态（等待某处将来的数据库操作任务请求）；如果缓存有任务，就取出一个来执行
// 当没有新的DBTask进队的时候，DBTask task = await self.Get(); 会一直处于等待状态。如果有新的DBTask了，则执行DBTask的任务，并且释放DBTask
                DBTask task = await self.Get(); 
                try {
                    await task.Run(); // 执行这个，异步数据库操作的任务，等待它执行完成
                }
                catch (Exception e) {
                    Log.Error(e);
                }
                task.Dispose(); // 执行完了，就可以回收这个数据库操作的任务了，前面不是看见它也有相当于是一个对象池来着？【对象池与否：再检查一下，先前的理解不一定正确】
            }
        }
    }

    // 用于管理DBTask的队列，先进先出。由于ET框架使用的是单线程，所以需要使用这种方式对DB任务进行处理。
    // 我把它理解为：DBTask 数据库操作任务的加工厂，是生产者提供方。感觉这个类基本上算是清楚了
    public sealed class DBTaskQueue : Component { // 关于异步任务的部分，数据库任务缓存壳加工厂：壳是对数据库异步任务的包装，感觉像是数据库异步任务的对象池
        public Queue<DBTask> queue = new Queue<DBTask>(); // 这里不曾说任务是单一的，可以混杂的，可是查询，增删查改等混杂的
        public TaskCompletionSource<DBTask> tcs;          // 还是看得狠昏：有个空的可以执行并写结果的壳？
        
// 当使用ADD时，会先判断一下this.tcs是否为空，【这是别人的注释，能看懂吗？】
// 如果不为空，那么说明有其他地方在queue队列为空时get一个任务，get为一个异步操作，如果队列为空，那么返回一个本实例tcs的ETTASK，让get的函数异步等待。
        public void Add(DBTask task) { // 这里是相对难理解一点儿的部分
// 在等待状态，如果我们往缓存队列里堆新的对象的时候，则会直接通过TaskCompletionSource.SetResult把想入队的DBTask直接返回。如果不在等待状态，则直接把DBTask入队。
            if (this.tcs != null) { // 不为空则有地方在等待get一个任务：那么直接将新增的task设置为tcs的结果，这样异步get会被唤醒，获取最新的Task。
                var t = this.tcs;
                this.tcs = null;   
                t.SetResult(task); // 直接执行当前任务：就是先前异步等待的任务，将会被吃唤醒，拿到它异步等待的任务
                return;
            }
            this.queue.Enqueue(task); // 如果不在等待状态，则直接把DBTask入队。
        }
        public Task<DBTask> Get() {
            // 如果缓存队列有，则返回出队列的对象。否则，则创建一个TaskCompletionSource，等待缓存队列有对象进入
            if (this.queue.Count > 0) {
                DBTask task = this.queue.Dequeue();
                return Task.FromResult(task);
            } // 下面的： 创建一个TaskCompletionSource，等待缓存队列有对象进入【可以参考，接上面的 Add() 方法来看，在等待状态。。。】
            TaskCompletionSource<DBTask> t = new TaskCompletionSource<DBTask>();
            this.tcs = t;
            return t.Task;
        }
    }
}