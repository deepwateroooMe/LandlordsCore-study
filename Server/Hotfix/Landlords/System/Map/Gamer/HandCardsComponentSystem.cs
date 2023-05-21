﻿using ETModel;
namespace ETHotfix {
    public static class HandCardsComponentSystem {
        // 获取所有手牌
        // <param name="self"></param>
        public static Card[] GetAll(this HandCardsComponent self) {
            return self.library.ToArray();
        }
        // 向牌库中添加牌
        // <param name="card"></param>
        public static void AddCard(this HandCardsComponent self, Card card) {
            self.library.Add(card);
        }
        // 出牌
        // <param name="self"></param>
        // <param name="card"></param>
        public static void PopCard(this HandCardsComponent self, Card card) {
            self.library.Remove(card);
        }
        // 手牌排序
        // <param name="self"></param>
        public static void Sort(this HandCardsComponent self) {
            CardsHelper.SortCards(self.library);
        }
    }
}
