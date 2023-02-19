using System.Collections.Generic;
namespace ETModel {

    // 牌库组件：这里，把牌库与玩家手牌的区别，弄清楚 
    public class DeckComponent : Component {
        // 牌库中的牌
        public readonly List<Card> library = new List<Card>();
        // 牌库中的总牌数
        public int CardsCount { get { return this.library.Count; } }
        
        public override void Dispose() {
            if(this.IsDisposed) {
                return;
            }
            base.Dispose();
            library.Clear();
        }
    }
}
