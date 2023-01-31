using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix {

 // Awake() Awake1() Start()    
    [ObjectSystem]
    public class MailBoxComponentAwakeSystem : AwakeSystem<MailBoxComponent> {
        public override void Awake(MailBoxComponent self) {
            self.ActorInterceptType = ActorInterceptType.None;
            self.Queue.Clear();
        }
    }
    [ObjectSystem]
    public class MailBoxComponentAwake1System : AwakeSystem<MailBoxComponent, string> {
        public override void Awake(MailBoxComponent self, string actorInterceptType) {
            self.ActorInterceptType = actorInterceptType;
            self.Queue.Clear();
        }
    }
    [ObjectSystem]
    public class MailBoxComponentStartSystem : StartSystem<MailBoxComponent> {
        public override void Start(MailBoxComponent self) {
            self.HandleAsync();
        }
    }

    // 挂上这个组件表示该Entity是一个Actor, 接收的消息将会队列处理
    public static class MailBoxComponentHelper {
        public static async Task AddLocation(this MailBoxComponent self) {
            await Game.Scene.GetComponent<LocationProxyComponent>().Add(self.Entity.Id, self.Entity.InstanceId);
        }
        public static async Task RemoveLocation(this MailBoxComponent self) {
            await Game.Scene.GetComponent<LocationProxyComponent>().Remove(self.Entity.Id);
        }
        public static void Add(this MailBoxComponent self, ActorMessageInfo info) {
            self.Queue.Enqueue(info);
            if (self.Tcs == null) {
                return;
            }
            var t = self.Tcs;
            self.Tcs = null;
            t.SetResult(self.Queue.Dequeue()); // 每添加一个任务，都需要处理一下当前的任务，设备结果，并置空
        }
        private static Task<ActorMessageInfo> GetAsync(this MailBoxComponent self) { // 队列中抓一个，或  来个新的作为当前待正在处理的任务
            if (self.Queue.Count > 0) {
                return Task.FromResult(self.Queue.Dequeue()); // 返回一个从当前队列中取出来的消息包装体
            }
            self.Tcs = new TaskCompletionSource<ActorMessageInfo>(); // 没有的话，就返回一个新生成的
            return self.Tcs.Task;
        }
        public static async void HandleAsync(this MailBoxComponent self) {
            ActorMessageDispatherComponent actorMessageDispatherComponent = Game.Scene.GetComponent<ActorMessageDispatherComponent>();
            
            long instanceId = self.InstanceId;
            
            while (true) {
                if (self.InstanceId != instanceId) { // 如果由于任何原因，发生了销毁等变化，就返回
                    return;
                }
                try {
                    ActorMessageInfo info = await self.GetAsync(); // 定义在上面
                    // 返回null表示actor已经删除,协程要终止
                    if (info.Message == null) {
                        return;
                    }
                    // 根据这个actor的类型分发给相应的ActorHandler处理
                    await actorMessageDispatherComponent.Handle(self, info);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }
    }
}