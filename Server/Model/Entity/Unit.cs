using MongoDB.Bson.Serialization.Attributes;
using PF;
namespace ETModel {
    // 始终不明白：这个到底是什么意思？可否理解为，游戏中的最小玩家单位？
    public enum UnitType {
        Hero,
        Npc
    }
    [ObjectSystem]
    public class UnitSystem : AwakeSystem<Unit, UnitType> {
        public override void Awake(Unit self, UnitType a) {
            self.Awake(a);
        }
    }
    public sealed class Unit: Entity {

        public UnitType UnitType { get; private set; }
        [BsonIgnore]
        public Vector3 Position { get; set; } // 带个位置信息，或是，玩家的当前位置，或是目标位置 ?
        
        public void Awake(UnitType unitType) {
            this.UnitType = unitType;
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}