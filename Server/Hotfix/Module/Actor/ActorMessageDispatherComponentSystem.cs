using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix {

    // Start() Load()    
    [ObjectSystem]
    public class ActorMessageDispatherComponentStartSystem: AwakeSystem<ActorMessageDispatherComponent> {
        public override void Awake(ActorMessageDispatherComponent self) {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class ActorMessageDispatherComponentLoadSystem: LoadSystem<ActorMessageDispatherComponent> {
        public override void Load(ActorMessageDispatherComponent self) {
            self.Load();
        }
    }

// Actor消息分发组件
    public static class ActorMessageDispatherComponentHelper {
        public static void Awake(this ActorMessageDispatherComponent self) {
            self.Load();
        }
        // 加载系统：就是说装载实例化了系统里可能会用到的两大类的处理器实例（一次性生成的所有的新的）
        public static void Load(this ActorMessageDispatherComponent self) {
            AppType appType = StartConfigComponent.Instance.StartConfig.AppType; // AppType的位操作，设计比较好
            self.ActorMessageHandlers.Clear();
            self.ActorTypeHandlers.Clear();
            List<Type> types = Game.EventSystem.GetTypes(typeof(ActorInterceptTypeHandlerAttribute));
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof(ActorInterceptTypeHandlerAttribute), false);
                if (attrs.Length == 0) {
                    continue;
                }
                ActorInterceptTypeHandlerAttribute actorInterceptTypeHandlerAttribute = (ActorInterceptTypeHandlerAttribute) attrs[0];
                if (!actorInterceptTypeHandlerAttribute.Type.Is(appType)) {
                    continue;
                }
                object obj = Activator.CreateInstance(type); // <<<<<<<<<<<<<<<<<<<< 创建了处理器的实例
                IActorInterceptTypeHandler iActorInterceptTypeHandler = obj as IActorInterceptTypeHandler;
                if (iActorInterceptTypeHandler == null) {
                    throw new Exception($"actor handler not inherit IEntityActorHandler: {obj.GetType().FullName}");
                }
                self.ActorTypeHandlers.Add(actorInterceptTypeHandlerAttribute.ActorType, iActorInterceptTypeHandler); // <<<<<<<<<< 
            }
            types = Game.EventSystem.GetTypes(typeof (ActorMessageHandlerAttribute));
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof(ActorMessageHandlerAttribute), false);
                if (attrs.Length == 0) {
                    continue;
                }
                
                ActorMessageHandlerAttribute messageHandlerAttribute = (ActorMessageHandlerAttribute) attrs[0];
                if (!messageHandlerAttribute.Type.Is(appType)) {
                    continue;
                }
                object obj = Activator.CreateInstance(type); // <<<<<<<<<<<<<<<<<<<< 
                IMActorHandler imHandler = obj as IMActorHandler;
                if (imHandler == null) {
                    throw new Exception($"message handler not inherit IMActorHandler abstract class: {obj.GetType().FullName}");
                }
                Type messageType = imHandler.GetMessageType();
                self.ActorMessageHandlers.Add(messageType, imHandler); // <<<<<<<<<< 
            }
        }

        public static async Task Handle(
            this ActorMessageDispatherComponent self, MailBoxComponent mailBoxComponent, ActorMessageInfo actorMessageInfo) {
            // 有拦截器使用拦截器处理: 有拦截器的意思是说，是内网消息，需要内网内先转发处理的吗？
            IActorInterceptTypeHandler iActorInterceptTypeHandler;
            if (self.ActorTypeHandlers.TryGetValue(mailBoxComponent.ActorInterceptType, out iActorInterceptTypeHandler)) {
                await iActorInterceptTypeHandler.Handle(actorMessageInfo.Session, mailBoxComponent.Entity, actorMessageInfo.Message);
                return;
            }
            
            // 没有拦截器就用IMActorHandler处理：这个标注是说，是服务器的内部消息，直接下发客户端？
            if (!self.ActorMessageHandlers.TryGetValue(actorMessageInfo.Message.GetType(), out IMActorHandler handler)) {
                throw new Exception($"not found message handler: {MongoHelper.ToJson(actorMessageInfo.Message)}");
            }
            await handler.Handle(actorMessageInfo.Session, mailBoxComponent.Entity, actorMessageInfo.Message);
        }
    }
}