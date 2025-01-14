﻿ namespace ETModel {

     // Map: 玩家分组，玩家对应于哪个房间？
     [ObjectSystem]
    public class GamerAwakeSystem : AwakeSystem<Gamer,long> {
        public override void Awake(Gamer self, long id) {
            self.Awake(id);
        }
    }
     // 房间玩家对象
    public sealed class Gamer : Entity {
        // 用户ID（唯一）
        public long UserID { get; private set; }
        // 玩家GateActorID
        public long PlayerID { get; set; }
        // 玩家所在房间ID
        public long RoomID { get; set; }
        // 是否准备
        public bool IsReady { get; set; } // 这里是因为，画面上有个准备按钮
        // 是否离线
        public bool isOffline { get; set; }
        public void Awake(long id) {
            this.UserID = id;
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.UserID = 0;
            this.PlayerID = 0;
            this.RoomID = 0;
            this.IsReady = false;
            this.isOffline = false;
        }
    }
}
