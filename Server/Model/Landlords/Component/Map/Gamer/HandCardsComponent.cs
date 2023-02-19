 using System.Collections.Generic;
namespace ETModel {
    // 游戏组件：每个玩家都有手牌
    public class HandCardsComponent : Component {
        // 所有手牌
        public readonly List<Card> library = new List<Card>();
        // 身份
        public Identity AccessIdentity { get; set; }
        // 是否托管：这里说的是，非地主，其它两个玩家，转让出牌权给系统吗？
        public bool IsTrusteeship { get; set; }
        // 手牌数
        public int CardsCount { get { return library.Count; } }
        public override void Dispose() {
            if(this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.library.Clear();
            AccessIdentity = Identity.None;
            IsTrusteeship = false;
        }
    }
}
