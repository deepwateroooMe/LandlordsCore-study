using System;
using ETModel;
namespace ETHotfix {
    public class InnerMessageDispatcher: IMessageDispatcher {

// 服务器之间的：内网消息分发器
        public void Dispatch(Session session, ushort opcode, object message) {
            // 收到actor消息,放入actor队列
            switch (message) {
                case IActorRequest iActorRequest: {
                    Entity entity = (Entity)Game.EventSystem.Get(iActorRequest.ActorId);
                    if (entity == null) { // 出错了，就回复一个提示出现了错误的消息回去
                        Log.Warning($"not found actor: {iActorRequest.ActorId}");
                        ActorResponse response = new ActorResponse {
                            Error = ErrorCode.ERR_NotFoundActor,
                            RpcId = iActorRequest.RpcId
                        };
                        session.Reply(response);
                        return;
                    }
                    MailBoxComponent mailBoxComponent = entity.GetComponent<MailBoxComponent>();
                    if (mailBoxComponent == null) { // 内网发消息，如果不具备发送消息的能力，就返回错误信息
                        ActorResponse response = new ActorResponse {
                            Error = ErrorCode.ERR_ActorNoMailBoxComponent,
                            RpcId = iActorRequest.RpcId
                        };
                        session.Reply(response);
                        Log.Error($"actor没有挂载MailBoxComponent组件: {entity.GetType().Name} {entity.Id}");
                        return;
                    }
                    mailBoxComponent.Add(new ActorMessageInfo() { Session = session, Message = iActorRequest });
                    return;
                }
                case IActorMessage iactorMessage: {
                    Entity entity = (Entity)Game.EventSystem.Get(iactorMessage.ActorId);
                    if (entity == null) {
                        Log.Error($"not found actor: {iactorMessage.ActorId}");
                        return;
                    }

                    MailBoxComponent mailBoxComponent = entity.GetComponent<MailBoxComponent>();
                    if (mailBoxComponent == null) {
                        Log.Error($"actor not add MailBoxComponent: {iactorMessage.ActorId}");
                        return;
                    }

                    mailBoxComponent.Add(new ActorMessageInfo() { Session = session, Message = iactorMessage });
                    return;
                }
                default: {
                    Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, new MessageInfo(opcode, message));
                    break;
                }
            }
        }
    }
}