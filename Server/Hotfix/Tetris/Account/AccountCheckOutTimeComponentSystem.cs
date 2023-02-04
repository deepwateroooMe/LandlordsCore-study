using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ET {

    [Timer(TimerType.AccountCheckOutTime)]
    public class AccountSessionCheckOutTimer : ATimer<AccountCheckOutTimeComponent> {
        public override void Run(AccountCheckOutTimeComponent self) {
            try {
                self.DeleteSession();
            } catch (Exception e) {
                Log.Error(e.ToString());
            }
        }
    }

// Awake() ?
    public class AccountCheckOutTimeComponentAwakeSystem : AwakeSystem<AccountCheckOutTimeComponent, long> {
        public override void Awake(AccountCheckOutTimeComponent self, long accountId) {
            self.AccountId = accountId;
            TimerComponent.Instance.Remove(ref self.Timer);
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 60000, TimerType.AccountSessionCheckOutTime, self);
        }
    }
    public class AccountCheckOutTimeComponentAwakeSystem : DestroySystem<AccountCheckOutTimeComponent> {
        public override void Destroy(AccountCheckOutTimeComponent self) {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }
    
    public static class AccountCheckOutTimeComponentSystem {

        public static void DeleteSession(this AccountCheckOutTimeComponent self) {
 // 从父物体上获取Session
            Session session = self.GetParent<Session>();
 // 拿到帐户注册登录服务器已经登录Session的instanceId
            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);
 // 比较父物体身上的InstanceId和AccountId对应的instanceid是不是同一个
            if (session.InstanceId == sessionInstanceId) {
 // 如果是同一个，那么就可以移除我们的注册登录服务器Session字典缓存的Session
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);
            }
 // 然后通知客户端断开连接
            session?.Send(new A2C_Disconnect(){ Error = 1 });
            session?.Disconnect().Coroutine();
        }
	}
}
