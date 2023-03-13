using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix {
    [ObjectSystem]
    public class ActorLocationSenderAwakeSystem : AwakeSystem<ActorLocationSender> {
        public override void Awake(ActorLocationSender self) {
            self.LastSendTime = TimeHelper.Now();
            self.Tcs = null;
            self.FailTimes = 0;
            self.ActorId = 0;
        }
    }
    [ObjectSystem]
    public class ActorLocationSenderStartSystem : StartSystem<ActorLocationSender> {
        public override async void Start(ActorLocationSender self) {
            self.ActorId = await Game.Scene.GetComponent<LocationProxyComponent>().Get(self.Id);
            self.Address = StartConfigComponent.Instance
                .Get(IdGenerater.GetAppIdFromId(self.ActorId))
                .GetComponent<InnerConfig>().IPEndPoint;
            self.UpdateAsync();
        }
    }
    [ObjectSystem]
    public class ActorLocationSenderDestroySystem : DestroySystem<ActorLocationSender> {
        public override void Destroy(ActorLocationSender self) {
            self.RunError(ErrorCode.ERR_ActorRemove);
            self.Id = 0;
            self.LastSendTime = 0;
            self.Address = null;
            self.ActorId = 0;
            self.FailTimes = 0;
            self.Tcs = null;
        }
    }
    public static class ActorLocationSenderHelper {
        private static void Add(this ActorLocationSender self, ActorTask task) {
            if (self.IsDisposed) {
                throw new Exception("ActorLocationSender Disposed! dont hold ActorMessageSender");
            }
            self.WaitingTasks.Enqueue(task);
            // failtimes > 0表示正在重试，这时候不能加到正在发送队列
            if (self.FailTimes == 0) { // 这里意思是说：如果已经重试到了最大次数，可以把结果写回去【以结束当前任务】？
                self.AllowGet();
            }
        }
        public static void RunError(this ActorLocationSender self, int errorCode) {
            while (self.WaitingTasks.Count > 0) {
                ActorTask actorTask = self.WaitingTasks.Dequeue();
                actorTask.Tcs?.SetException(new RpcException(errorCode, ""));
            }
            self.WaitingTasks.Clear();
        }
        private static void AllowGet(this ActorLocationSender self) {
            if (self.Tcs == null || self.WaitingTasks.Count <= 0) { // 队列中没有元素或者Tcs为空，直接返回
                return;
            }
            ActorTask task = self.WaitingTasks.Peek(); // 这里只是 Peek() 没有拿出来，写结果的时候才拿出来
            var t = self.Tcs;
            self.Tcs = null;
            t.SetResult(task); // 设置回调，ActorLocationSender中的任务结束
        }
        private static Task<ActorTask> GetAsync(this ActorLocationSender self) { // 从队列中获取对头元素，或者创建一个
            if (self.WaitingTasks.Count > 0) {
                ActorTask task = self.WaitingTasks.Peek();
                return Task.FromResult(task);
            }
            self.Tcs = new TaskCompletionSource<ActorTask>();
            return self.Tcs.Task;
        }
        public static async void UpdateAsync(this ActorLocationSender self) {
            try {
                long instanceId = self.InstanceId;
                while (true) {
                    if (self.InstanceId != instanceId) {
                        return;
                    }
                    ActorTask actorTask = await self.GetAsync(); // 从队列中获取对头元素，或者创建一个
                    if (self.InstanceId != instanceId) {
                        return;
                    }
                    if (actorTask.ActorRequest == null) {
                        return;
                    }
                    await self.RunTask(actorTask); // 异步等待消送和返回
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        private static async Task RunTask(this ActorLocationSender self, ActorTask task) {
            ActorMessageSender actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(self.ActorId);
            IActorResponse response = await actorMessageSender.Call(task.ActorRequest);
            // 发送成功
            switch (response.Error) {
                case ErrorCode.ERR_NotFoundActor:
                    // 如果没找到Actor,重试
                    ++self.FailTimes;
                    // 失败MaxFailTimes次则清空actor发送队列，返回失败
                    if (self.FailTimes > ActorLocationSender.MaxFailTimes) {
                        // 失败直接删除actorproxy
                        Log.Info($"actor send message fail, actorid: {self.Id}");
                        self.RunError(response.Error);
                        self.GetParent<ActorLocationSenderComponent>().Remove(self.Id);
                        return;
                    }
                    // 等待0.5s再发送
                    await Game.Scene.GetComponent<TimerComponent>().WaitAsync(500);
                    self.ActorId = await Game.Scene.GetComponent<LocationProxyComponent>().Get(self.Id);
                    self.Address = StartConfigComponent.Instance
                        .Get(IdGenerater.GetAppIdFromId(self.ActorId))
                        .GetComponent<InnerConfig>().IPEndPoint;
                    self.AllowGet();
                    return;
                case ErrorCode.ERR_ActorNoMailBoxComponent:
                    self.RunError(response.Error);
                    self.GetParent<ActorLocationSenderComponent>().Remove(self.Id);
                    return;
                default: // 异步发送任务执行成功：
                    self.LastSendTime = TimeHelper.Now();
                    self.FailTimes = 0; // 就可以允许写结果了？
                    self.WaitingTasks.Dequeue(); // 是结果出来的时候，再从队列中取出来，队列中的任务是一个一个地执行
                    if (task.Tcs == null) {
                        return;
                    }
                    IActorLocationResponse actorLocationResponse = response as IActorLocationResponse;
                    if (actorLocationResponse == null) {
                        task.Tcs.SetException(new Exception($"actor location respose is not IActorLocationResponse, but is: {response.GetType().Name}"));
                    }
                    task.Tcs.SetResult(actorLocationResponse);
                    return;
            }
        }
        public static void Send(this ActorLocationSender self, IActorLocationMessage request) {
            if (request == null) {
                throw new Exception($"actor location send message is null");
            }
            ActorTask task = new ActorTask(request);
            self.Add(task);
        }
        public static Task<IActorLocationResponse> Call(this ActorLocationSender self, IActorLocationRequest request) {
            if (request == null) {
                throw new Exception($"actor location call message is null");
            } 
            TaskCompletionSource<IActorLocationResponse> tcs = new TaskCompletionSource<IActorLocationResponse>();
            ActorTask task = new ActorTask(request, tcs); // 先包装一个异步任务；再包装一个 ActorTask. 都是封装
            self.Add(task);
            return task.Tcs.Task;
        }
    }
}