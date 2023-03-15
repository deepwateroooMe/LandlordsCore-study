using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {

// 中央邮政，或是位置情报局：比较好懂，只是下面找个定义看下
    public abstract class LocationTask: Component {
        public abstract void Run(); // 找具体的实现定义: 下面的， LocationQueryTask 等
    }
    
    [ObjectSystem]
    public class LocationQueryTaskAwakeSystem : AwakeSystem<LocationQueryTask, long> {
        public override void Awake(LocationQueryTask self, long key) {
            self.Key = key;
            self.Tcs = new TaskCompletionSource<long>();
        }
    }
    
    public sealed class LocationQueryTask : LocationTask {
        public long Key;
        public TaskCompletionSource<long> Tcs;
        public Task<long> Task {
            get {
                return this.Tcs.Task;
            }
        }
        public override void Run() {
            try {
                LocationComponent locationComponent = this.GetParent<LocationComponent>();
                long location = locationComponent.Get(this.Key);
                this.Tcs.SetResult(location);
            }
            catch (Exception e) {
                this.Tcs.SetException(e);
            }
        }
    }
    // 位置组件：统管所有注册过的玩家位置等，中央邮政系统，任何玩家的搬移，向这里汇报， single.point.of.Truth
    public class LocationComponent : Component {
        private readonly Dictionary<long, long> locations = new Dictionary<long, long>(); // 全局管理的：最新状态位置字典，实时更新
        private readonly Dictionary<long, long> lockDict = new Dictionary<long, long>();  // 搬迁中：锁定状态管理
        private readonly Dictionary<long, Queue<LocationTask>> taskQueues = new Dictionary<long, Queue<LocationTask>>(); // 搬迁中：回调管理，待搬迁好了会回调

        public void Add(long key, long instanceId) {
            this.locations[key] = instanceId;
            Log.Info($"location add key: {key} instanceId: {instanceId}");
            // 更新db
            // await Game.Scene.GetComponent<DBProxyComponent>().Save(new Location(key, address));
        }
        public void Remove(long key) {
            Log.Info($"location remove key: {key}");
            this.locations.Remove(key);
        }
        public long Get(long key) {
            this.locations.TryGetValue(key, out long instanceId);
            return instanceId;
        }
        public async void Lock(long key, long instanceId, int time = 0) {
            if (this.lockDict.ContainsKey(key)) {
                Log.Error($"不可能同时存在两次lock, key: {key} InstanceId: {instanceId}");
                return;
            }
            Log.Info($"location lock key: {key} InstanceId: {instanceId}");
            if (!this.locations.TryGetValue(key, out long saveInstanceId)) {
                Log.Error($"actor没有注册, key: {key} InstanceId: {instanceId}");
                return;
            }
            if (saveInstanceId != instanceId) {
                Log.Error($"actor注册的instanceId与lock的不一致, key: {key} InstanceId: {instanceId} saveInstanceId: {saveInstanceId}");
                return;
            }
// 这里，小伙伴要搬迁在搬迁了，但是原纪录 locations 字典里并没有移除。所以搬迁过程中，会有消息邮件发送了过去的旧地址？不会发旧地址。只要它搬迁前申明过它要搬家，就不会发错误发
            this.lockDict.Add(key, instanceId);
            // 超时则解锁【前面是原注释？】若是设置了等待时间，
            if (time > 0) {
                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(time); // 这里就会等那么久
                if (!this.lockDict.ContainsKey(key)) {
                    return;
                }
                Log.Info($"location timeout unlock key: {key} time: {time}");
                this.UnLock(key); // 时间到了，解锁：执行并返回所有对这个 key 有兴趣，索要过地址，还缓存着没能处理的异步位置请求消息
            }
        }
        // 上下文场景：给了小伙伴搬迁时间；等它搬家搬完了，换玩场景了，会返回它的最新位置，供中央邮政统计管理 
        public void UnLockAndUpdate(long key, long oldInstanceId, long instanceId) {
            this.lockDict.TryGetValue(key, out long lockInstanceId);
            if (lockInstanceId != oldInstanceId) {
                Log.Error($"unlock appid is different {lockInstanceId} {oldInstanceId}" );
            }
            Log.Info($"location unlock key: {key} oldInstanceId: {oldInstanceId} new: {instanceId}");
            this.locations[key] = instanceId; // 更新到统计库里
            this.UnLock(key); // 解锁当前的 Key: 执行相关必要的回调【若是有这个 Key 相关的任务在等待】
        }
        private void UnLock(long key) { // 就是执行，先前搬家过程中注册过的异步索求位置消息的回复消息
            this.lockDict.Remove(key); // 首行，从锁定倦态解锁
            if (!this.taskQueues.TryGetValue(key, out Queue<LocationTask> tasks)) {
                return;
            }
            while (true) { //tasks: 就是排着长队索要这个搬迁过程中小伙伴位置的异步位置请求任务；现在既然它搬玩了，也更新了，就一一告诉那些想给要它位置的异步请求任务
                if (tasks.Count <= 0) { // 任务为空：队列里的任务，终于执行完了，从这里返回
                    this.taskQueues.Remove(key);
                    return;
                }
                if (this.lockDict.ContainsKey(key)) { // 小伙伴搬家【这里更像是，过程中又发生了再一次地搬迁？】，更新场景中，仍处于被锁住的状态，关切的小伙伴们需要等候。。。。。
                    return;
                }
                LocationTask task = tasks.Dequeue(); // 一个个地遍历，将任务全部执行完成
                try {
                    task.Run();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
                task.Dispose(); // 任务完成了，就回收
            }
        }
        public Task<long> GetAsync(long key) {
            if (!this.lockDict.ContainsKey(key)) { // 先检查：它正在搬迁吗？没有。没有搬迁，不在搬迁，直接返回位置消息
                this.locations.TryGetValue(key, out long instanceId); // 全局最新状态数据字典中去读
                Log.Info($"location get key: {key} {instanceId}");
                return Task.FromResult(instanceId); // 直接返回结果
            } // 下面：正在搬迁，就把异步位置请求消息的任务先缓存起来，等它搬迁完成
            LocationQueryTask task = ComponentFactory.CreateWithParent<LocationQueryTask, long>(this, key); // 被发消息的小伙伴正在搬家，缓存任务，请等待。。。。。
            this.AddTask(key, task);
            return task.Task;
        }
        public void AddTask(long key, LocationTask task) {
            if (!this.taskQueues.TryGetValue(key, out Queue<LocationTask> tasks)) {
                tasks = new Queue<LocationTask>();
                this.taskQueues[key] = tasks;
            }
            tasks.Enqueue(task);
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.locations.Clear();
            this.lockDict.Clear();
            this.taskQueues.Clear();
        }
    }
}