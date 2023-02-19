 using System.Collections.Generic;
namespace ETModel {

    // 在线组件，用于记录在线玩家。这里是 Realm 注册登录服的组件，它身背一个本地缓存字典
    public class OnlineComponent : Component {
        private readonly Dictionary<long, int> dictionary = new Dictionary<long, int>();
        // 添加在线玩家
        public void Add(long userId, int gateAppId) {
            dictionary.Add(userId, gateAppId);
        }
        // 获取在线玩家网关服务器ID
        public int Get(long userId) {
            int gateAppId;
            dictionary.TryGetValue(userId, out gateAppId);
            return gateAppId;
        }
        // 移除在线玩家
        public void Remove(long userId) {
            dictionary.Remove(userId);
        }
    }
}
