using System;
using System.Threading.Tasks;
namespace ET {// 这是我参照其它搬过来的，可能方法定义不对
    public interface IMActorHandler {
        Task Handle(Session session, Entity entity, object actorMessage);
        Type GetMessageType();
    }
}


