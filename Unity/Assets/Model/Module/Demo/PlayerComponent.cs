using System.Collections.Generic;
using System.Linq;

namespace ETModel {

    [ObjectSystem]
    public class PlayerComponentAwakeSystem : AwakeSystem<PlayerComponent> {
        public override void Awake(PlayerComponent self) {
            self.Awake();
        }
    }

 // 玩家组件: 是对所有玩家的总管吗?    
    public class PlayerComponent : Component {

        public static PlayerComponent Instance { get; private set; }
        public Player MyPlayer;
        
        private readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>(); // 它手下管了一堆玩家信息

        public void Awake() {
            Instance = this;
        }
        
        public void Add(Player player) {
            this.idPlayers.Add(player.Id, player);
        }
        public Player Get(long id) {
            Player player;
            this.idPlayers.TryGetValue(id, out player);
            return player;
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