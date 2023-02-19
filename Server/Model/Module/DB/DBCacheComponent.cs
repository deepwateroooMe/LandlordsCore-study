using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace ETModel {
    [ObjectSystem]
    public class DbCacheComponentSystem : AwakeSystem<DBCacheComponent> {
        public override void Awake(DBCacheComponent self) {
            self.Awake();
        }
    }
// 用来缓存数据操作异步任务: 这里感觉稍微有点儿陌生，主要是MongoDB的集群缓存机制自己还不懂，还没细读源码，不知道说的是不是一回事儿？不是一回事儿?好像是，所以没有理解透彻
    public class DBCacheComponent : Component {

        // 表名，表中所有条目存字典里
        public Dictionary<string, Dictionary<long, ComponentWithId>> cache = new Dictionary<string, Dictionary<long, ComponentWithId>>();
        // ET中一共定有32条DBTaskQueue，如果数量不够的话，有些数据库操作耗时很长，这样其他数据库任务会等待太久才能执行。而数据库本身是多线程的操作，所以可以将任务分为32条，防止全部等待某一条操作耗时太久。
        public const int taskCount = 32; // 是32 条线程来多线程处理。【想的是对的】
        public List<DBTaskQueue> tasks = new List<DBTaskQueue>(taskCount); // 可以有包含32个任务队列的链表

        public void Awake() {
            for (int i = 0; i < taskCount; ++i) {
                DBTaskQueue taskQueue = ComponentFactory.Create<DBTaskQueue>(); // 这里，是从对象池里去抓的
                this.tasks.Add(taskQueue);
            }
        }
        // 之前我们提到的AddToCache跟GetFromCache，都只是针对缓存的操作，那我们怎么对数据库操作呢？着就要用到我们的DBTask了。
        public Task<bool> Add(ComponentWithId component, string collectionName = "") { // 并没有添加到缓存 cache: 可能这是新版本的更新，并没有添加到缓存 ?
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(); // 添加任务：需要返回一个添加成功与否的状态值。要异步拿到这个值，就要封装成这个样子
            if (string.IsNullOrEmpty(collectionName)) {
                collectionName = component.GetType().Name;
            } // 下面：创建一个异步保存到数据库的任务
            DBSaveTask task = ComponentFactory.CreateWithId<DBSaveTask, ComponentWithId, string, TaskCompletionSource<bool>>(component.Id, component, collectionName, tcs);
            // 将添加到数据库的异步任务：缓存起来，缓存到由 32 条线程来多线程处理的缓存队列中去，等待被执行
            this.tasks[(int)((ulong)task.Id % taskCount)].Add(task); // 逻辑简单粗暴：按顺序，32 一个一个地加。实际中可以有很多优化？队列预估的任务执行等待时间？
            return tcs.Task; // 返回异步等待任务执行的结果 
        }
        public Task<bool> AddBatch(List<ComponentWithId> components, string collectionName) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            DBSaveBatchTask task = ComponentFactory.Create<DBSaveBatchTask, List<ComponentWithId>, string, TaskCompletionSource<bool>>(components, collectionName, tcs);
            this.tasks[(int)((ulong)task.Id % taskCount)].Add(task); 
            return tcs.Task;
        }

 // AddToCache() GetFromCache() RemoveFromCache(): 三个缓存里的操作：他们的作用的对象是缓存 cache, 并不是真正要去直接操作数据库 ?
        public void AddToCache(ComponentWithId component, string collectionName = "") { // 唯一对cache进行添加操作的方法
            if (string.IsNullOrEmpty(collectionName)) { // 这个名字：怎么背着数据库表名的信息？没明白
                collectionName = component.GetType().Name;
            }
            Dictionary<long, ComponentWithId> collection; // 存储的是，一个表的所有信息。一个表可以有很多条，每条有独特 id long
            if (!this.cache.TryGetValue(collectionName, out collection)) {
                collection = new Dictionary<long, ComponentWithId>();
                this.cache.Add(collectionName, collection);
            }
            collection[component.Id] = component;
        }
        public ComponentWithId GetFromCache(string collectionName, long id) {
            Dictionary<long, ComponentWithId> d;
            if (!this.cache.TryGetValue(collectionName, out d)) {
                return null;
            }
            ComponentWithId result;
            if (!d.TryGetValue(id, out result)) {
                return null;
            }
            return result;
        }
        public void RemoveFromCache(string collectionName, long id) {
            Dictionary<long, ComponentWithId> d;
            if (!this.cache.TryGetValue(collectionName, out d)) {
                return;
            }
            d.Remove(id);
        }

// Get() GetBatch() 这几个函数，它们获取数据的源头，首先去检查能否从缓存中直接读取，能，就直接返回结果。不能或是不能读全读，就要通过异步任务去从源数据库中读取
// GetJson(): 这个方法是，直接去操作数据库的，而不是先试图从缓存中读取
         public Task<ComponentWithId> Get(string collectionName, long id) {
            ComponentWithId component = GetFromCache(collectionName, id); // 这里，是先试图从缓存中直接读取
            if (component != null) { // 非空，读到了，就直接返回。从缓存里读(安全吗，数据一致吗？) 【单线程逻辑，从字典缓存里读是安全的；目前只有这一个缓存？就是最新数据，也是数据一致的。这里缓存的数据可能只是在等待某个空闲的时候将数据写进数据库而已】
                return Task.FromResult(component); // 意思是说，把拿到的结果写进一个可以返回的任务里，直接返回
            }
 //　还是说，这里的缓存不止一个？因为下面，仍然是试图从缓存中去读。还是说，这里读不到读到的为空，稍微等会儿再去读？这里没弄明白
            TaskCompletionSource<ComponentWithId> tcs = new TaskCompletionSource<ComponentWithId>(); // 任务类型申明清楚了
             // 创建了这个查询异步任务的包装，是对远程数据库中心服的查询操作，会异步返回结果。这里创建不是new,是工厂化生产的
            DBQueryTask dbQueryTask = ComponentFactory.CreateWithId<DBQueryTask, string, TaskCompletionSource<ComponentWithId>>(id, collectionName, tcs);
            this.tasks[(int)((ulong)id % taskCount)].Add(dbQueryTask); // 这里就是随机分配给，32个中的某个缓存队列(应该是有个线程去处理每个缓存队列里的任务)
            return tcs.Task;
        }
        public Task<List<ComponentWithId>> GetBatch(string collectionName, List<long> idList) {
            List <ComponentWithId> components = new List<ComponentWithId>();
            bool isAllInCache = true;
            foreach (long id in idList) {
                ComponentWithId component = this.GetFromCache(collectionName, id); // 如果缓存中有我们想要的数据，则直接从缓存中获取，否则，尝试从数据库中获取。
                if (component == null) {
                    isAllInCache = false;
                    break;
                }
                components.Add(component);
            }
            if (isAllInCache) {
                return Task.FromResult(components); // 这里什么情况下可以直接返回，就是从缓存里可以拿到完备数据的时候，那么不同线程拿到的结果一致吗？不同线程从(不同？)缓存里读，能够保证读一致性吗？这些需要去理解
            }
 // 这下面好像是说：当从缓存中？拿不到完整数据的时候，会试图从主版本去试图拿到可能更为完备的数据？明天早上要补下理论
            TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();
            DBQueryBatchTask dbQueryBatchTask = ComponentFactory.Create<DBQueryBatchTask, List<long>, string, TaskCompletionSource<List<ComponentWithId>>>(idList, collectionName, tcs);
            this.tasks[(int)((ulong)dbQueryBatchTask.Id % taskCount)].Add(dbQueryBatchTask); // 这些包装的异步任务缓存待执行
            return tcs.Task;
        }
        public Task<List<ComponentWithId>> GetJson(string collectionName, string json) {
            TaskCompletionSource<List<ComponentWithId>> tcs = new TaskCompletionSource<List<ComponentWithId>>();
            DBQueryJsonTask dbQueryJsonTask = ComponentFactory.Create<DBQueryJsonTask, string, string, TaskCompletionSource<List<ComponentWithId>>>(collectionName, json, tcs);
            this.tasks[(int)((ulong)dbQueryJsonTask.Id % taskCount)].Add(dbQueryJsonTask);
            return tcs.Task;
        }
    }
}
