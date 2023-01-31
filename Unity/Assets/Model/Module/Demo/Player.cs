namespace ETModel {
    public sealed class Player : Entity {

 // UnitId 与 Id,有什么区别?        
        public long UnitId { get; set; } // 这里还没想明白: 为什么就一定要背个UnitId,与其它instanceId有什么区别是?
        
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}