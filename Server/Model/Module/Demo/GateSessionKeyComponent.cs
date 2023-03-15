using System.Collections.Generic;
namespace ETModel {

    public class GateSessionKeyComponent : Component {
        // 把与网关服通信的会话框客户端端 Key, 与客户端的登录帐户联系起来。等会话框超时，会话框自动销毁时，需要通知登录帐户自动下线？还是说？。。。。。重新创建当前登录帐户的新的会话框呢？
        private readonly Dictionary<long, string> sessionKey = new Dictionary<long, string>();
        
        public void Add(long key, string account) {
            this.sessionKey.Add(key, account);
            this.TimeoutRemoveKey(key);
        }
        public string Get(long key) {
            string account = null;
            this.sessionKey.TryGetValue(key, out account);
            return account;
        }
        public void Remove(long key) {
            this.sessionKey.Remove(key);
        }
        private async void TimeoutRemoveKey(long key) {
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000); // 定义一个有效 Key 的有效时间是20 秒
            this.sessionKey.Remove(key);
        }
    }
}
