using System.Collections.Generic;
namespace ETModel {

    // 这里具体又定义了一个游戏子类，感觉跟前面定义的是重复的。但是当时自己练习时加的，可能可以不必要，可以删除
    public class LandlordsGateSessionKeyComponent : Component {

        private readonly Dictionary<long, long> sessionKey = new Dictionary<long, long>();

        public void Add(long key, long userId) {
            this.sessionKey.Add(key, userId);
            this.TimeoutRemoveKey(key);
        }

        public long Get(long key) {
            long userId;
            this.sessionKey.TryGetValue(key, out userId);
            return userId;
        }

        public void Remove(long key) {
            this.sessionKey.Remove(key);
        }

        private async void TimeoutRemoveKey(long key) {
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000);
            this.sessionKey.Remove(key);
        }
    }
}
