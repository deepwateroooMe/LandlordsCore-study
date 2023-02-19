using System;
using System.Collections.Generic;
using ETModel;
namespace ETHotfix {
    [ObjectSystem]
    public class DeckComponentAwakeSystem : AwakeSystem<DeckComponent> {
        public override void Awake(DeckComponent self) {
            self.Awake();
        }
    }
    public static class DeckComponentSystem {
        public static void Awake(this DeckComponent self) {
            self.CreateDeck();
        }
        // 洗牌
        public static void Shuffle(this DeckComponent self) { // 就是把牌，根据随机数，再重新排列一遍
            if (self.CardsCount == 54) {
                Random random = new Random();
                List<Card> newCards = new List<Card>();
                foreach (var card in self.library) {
                    newCards.Insert(random.Next(newCards.Count + 1), card);
                }
                self.library.Clear();
                self.library.AddRange(newCards);
            }
        }
        // 发牌
        public static Card Deal(this DeckComponent self) {
            Card card = self.library[self.CardsCount - 1];
            self.library.Remove(card);
            return card;
        }
        // 向牌库中添加牌
        // <param name="card"></param>
        public static void AddCard(this DeckComponent self, Card card) {
            self.library.Add(card);
        }
        // 创建一副牌: 52 张牌
        private static void CreateDeck(this DeckComponent self) {
            // 创建普通扑克
            for (int color = 0; color < 4; color++) {
                for (int value = 0; value < 13; value++) {
                    Weight w = (Weight)value;
                    Suits s = (Suits)color;
                    Card card = new Card() { CardSuits = s, CardWeight = w };
                    self.library.Add(card);
                }
            }
            // 创建大小王扑克
            self.library.Add(new Card() { CardWeight = Weight.Sjoker, CardSuits = Suits.None });
            self.library.Add(new Card() { CardWeight = Weight.Ljoker, CardSuits = Suits.None });
        }
    }
}
