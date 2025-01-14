﻿using System;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix {

    // gate session 拦截器，收到的actor消息直接转发给客户端
    [ActorInterceptTypeHandler(AppType.Gate, ActorInterceptType.GateSession)] // 这是 ActorInterceptTypeHandler 标签的一个实际使用的例子
    public class GateSessionActorInterceptInterceptTypeHandler : IActorInterceptTypeHandler {

        public async Task Handle(Session session, Entity entity, object actorMessage) {
            try {
                IActorMessage iActorMessage = actorMessage as IActorMessage;
                // 发送给客户端
                Session clientSession = entity as Session;
                iActorMessage.ActorId = 0;
                clientSession.Send(iActorMessage);
                await Task.CompletedTask;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
