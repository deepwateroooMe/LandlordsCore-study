namespace ETModel {
    public class GameControllerComponent : Component {
        // 房间配置
        public RoomConfig Config { get; set; }
        // 底分：是什么？
        public long BasePointPerMatch { get; set; }
        // 全场倍率：找下这个倍率的计算方式，影响什么？
        public int Multiples { get; set; }
        // 最低入场门槛：理解一下
        public long MinThreshold { get; set; }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.BasePointPerMatch = 0;
            this.Multiples = 0;
            this.MinThreshold = 0;
        }
    }
}
