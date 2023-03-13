using PF;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
namespace ETModel {

    public enum UnitType { // 这里说的是角色类型了吧
        Hero,
        Npc
    }
    public sealed class Unit: Entity {
        public GameObject GameObject;
        
        public void Awake() {
        }
// 带着位置，与旋转信息
        public Vector3 Position {
            get {
                return GameObject.transform.position;
            }
            set {
                GameObject.transform.position = value;
            }
        }
        public Quaternion Rotation {
            get {
                return GameObject.transform.rotation;
            }
            set {
                GameObject.transform.rotation = value;
            }
        }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}