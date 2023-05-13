﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix {

    [ActorMessageHandler(AppType.Map)] // 【去弄吃的，下次从这里接着往后看。爱表哥，爱生活！！！】
    public class Actor_GamerPlayCard_ReqHandler : AMActorRpcHandler<Gamer, Actor_GamerPlayCard_Req, Actor_GamerPlayCard_Ack> {

        protected override async Task Run(Gamer gamer, Actor_GamerPlayCard_Req message, Action<Actor_GamerPlayCard_Ack> reply) {
            Actor_GamerPlayCard_Ack response = new Actor_GamerPlayCard_Ack();
            try {
                Room room = Game.Scene.GetComponent<RoomComponent>().Get(gamer.RoomID);
                GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();
                DeskCardsCacheComponent deskCardsCache = room.GetComponent<DeskCardsCacheComponent>();
                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
                // 检测是否符合出牌规则
                if (CardsHelper.PopEnable(message.Cards, out CardsType type)) {
                    // 当前出牌牌型是否比牌桌上牌型的权重更大
                    bool isWeightGreater = CardsHelper.GetWeight(message.Cards, type) > deskCardsCache.GetTotalWeight();
                    // 当前出牌牌型是否和牌桌上牌型的数量一样
                    bool isSameCardsNum = message.Cards.count == deskCardsCache.GetAll().Length;
                    // 当前出牌玩家是否是上局最大出牌者: 为什么是这里来检查这个呢？
                    // 这个，如果是上局最大权重玩家的先手出牌，它将会有选择权，可以作为新轮次第一个出牌者出任意牌型；而其它跟随出牌的，必须遵循这个出牌牌型 
                    bool isBiggest = orderController.Biggest == orderController.CurrentAuthority;
                    // 当前牌桌牌型是否是顺子
                    bool isStraight = deskCardsCache.Rule == CardsType.Straight || deskCardsCache.Rule == CardsType.DoubleStraight || deskCardsCache.Rule == CardsType.TripleStraight;
                    // 当前出牌牌型是否和牌桌上牌型一样
                    bool isSameCardsType = type == deskCardsCache.Rule;
                    if (isBiggest ||    // 先手出牌玩家
                        type == CardsType.JokerBoom ||  // 王炸
                        type == CardsType.Boom && isWeightGreater ||    // 更大的炸弹
                        isSameCardsType && isStraight && isSameCardsNum && isWeightGreater ||   // 更大的顺子
                        isSameCardsType && isWeightGreater) {      // 更大的同类型牌
                        // 下面定义了这个斗地主游戏的：各种花式赌注玩法
                        if (type == CardsType.JokerBoom) {
                            // 王炸翻4倍
                            gameController.Multiples *= 4; // 这些翻倍了之后，最终影响的应该是，地主或是相应玩家赢的时候，赢或是输的钱数？
                            room.Broadcast(new Actor_SetMultiples_Ntt() { Multiples = gameController.Multiples });
                        } else if (type == CardsType.Boom) {
                            // 炸弹翻2倍
                            gameController.Multiples *= 2; // 这些翻倍了之后，最终影响的应该是，地主或是相应玩家赢的时候，赢或是输的钱数？
                            room.Broadcast(new Actor_SetMultiples_Ntt() { Multiples = gameController.Multiples });
                        }
                    } else {
                        response.Error = ErrorCode.ERR_PlayCardError;
                        reply(response);
                        return;
                    }
                } else {
                    response.Error = ErrorCode.ERR_PlayCardError;
                    reply(response);
                    return;
                }
                // 如果符合将牌从手牌移到出牌缓存区
                deskCardsCache.Clear();
                deskCardsCache.Rule = type;
                HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                foreach (var card in message.Cards) {
                    handCards.PopCard(card);
                    deskCardsCache.AddCard(card);
                }
                reply(response);
                // 转发玩家出牌消息
                room.Broadcast(new Actor_GamerPlayCard_Ntt() { UserID = gamer.UserID, Cards = message.Cards });
                // 游戏控制器继续游戏
                gameController.Continue(gamer);
            }
            catch (Exception e) {
                ReplyError(response, e, reply);
            }
        }
    }
}
