﻿ namespace ETModel {
     // 网关服，这里用户的相关信息
     [ObjectSystem]
    public class UserAwakeSystem : AwakeSystem<User,long> {
        public override void Awake(User self, long id) {
            self.Awake(id);
        }
    }
    // 玩家对象
    public sealed class User : Entity {
        // 用户ID（唯一）
        public long UserID { get; private set; }
        // 是否正在匹配中：是因为游戏需要至少三个玩家才能成一桌
        public bool IsMatching { get; set; }
        // Gate转发ActorID
        public long ActorID { get; set; }
        public void Awake(long id) {
            this.UserID = id;
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.IsMatching = false;
            this.ActorID = 0;
        }
    }
}