namespace ETModel {
    public sealed class Player : Entity {

 // UnitId 与 Id,有什么区别? 任何实例，都有 id 来区分其它实例； UnitId, 暂时把它理解为什么最小单位，带游戏中角色的最基本 position 等信息？        
        public long UnitId { get; set; } // 这里还没想明白: 为什么就一定要背个UnitId ？
        
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}