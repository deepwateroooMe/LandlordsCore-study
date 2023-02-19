using System.Collections.Generic;
using System.Linq;
namespace ETModel {
    [ObjectSystem]
    public class PlayerComponentSystem : AwakeSystem<PlayerComponent> {
        public override void Awake(PlayerComponent self) {
            self.Awake();
        }
    }
    public class PlayerComponent : Component {
        public static PlayerComponent Instance { get; private set; } // 当玩家组件，静态单例，它就要负责管理所有的玩家
        public Player MyPlayer; // <<<<<<<<<<<<<<<<<<<< ？？？它比较特殊：去找到它的使用赋值的地方
        
        private readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();
        public void Awake() {
            Instance = this;
        }
        
        public void Add(Player player) {
            this.idPlayers.Add(player.Id, player);
        }
        public Player Get(long id) {
            this.idPlayers.TryGetValue(id, out Player gamer);
            return gamer;
        }
        public void Remove(long id) {
            this.idPlayers.Remove(id);
        }
        public int Count {
            get {
                return this.idPlayers.Count;
            }
        }
        public Player[] GetAll() {
            return this.idPlayers.Values.ToArray();
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            
            base.Dispose();
            foreach (Player player in this.idPlayers.Values) {
                player.Dispose();
            }
            Instance = null;
        }
    }
}