 using System;
using System.Collections.Generic;
namespace ETModel {

    [ObjectSystem]
    public class MessageDispatherComponentAwakeSystem : AwakeSystem<MessageDispatherComponent> {
        public override void Awake(MessageDispatherComponent t) {
            t.Awake();
        }
    }
    [ObjectSystem]
    public class MessageDispatherComponentLoadSystem : LoadSystem<MessageDispatherComponent> {
        public override void Load(MessageDispatherComponent self) {
            self.Load();
        }
    }
    // 消息分发组件
    public class MessageDispatherComponent : Component {
        private readonly Dictionary<ushort, List<IMHandler>> handlers = new Dictionary<ushort, List<IMHandler>>();
        public void Awake() {
            this.Load();
        }
        public void Load() {
            this.handlers.Clear();
            List<Type> types = Game.EventSystem.GetTypes(typeof(MessageHandlerAttribute)); // 标注系统：同消息一样，标注类型，扫描标签，归类和创建实例。
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0) {
                    continue;
                }
                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null) {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }
                Type messageType = iMHandler.GetMessageType();
                // 这里有点儿迷糊： this.Entity 指的是【这个组件，它的父控件 as-Entity】 ETModel-Game.Scene.AddComponent<OpcodeTypeComponent>
                ushort opcode = this.Entity.GetComponent<OpcodeTypeComponent>().GetOpcode(messageType); // 这里它们说的应该是同一个组件
                if (opcode == 0) {
                    Log.Error($"消息opcode为0: {messageType.Name}");
                    continue;
                }
                this.RegisterHandler(opcode, iMHandler);
            }
        }
        public void RegisterHandler(ushort opcode, IMHandler handler) { // 将一类 opcode 的消息处理器，加入到相应的链表中去
            if (!this.handlers.ContainsKey(opcode)) {
                this.handlers.Add(opcode, new List<IMHandler>());
            }
            this.handlers[opcode].Add(handler);
        }

        public void Handle(Session session, MessageInfo messageInfo) {
            List<IMHandler> actions;
            if (!this.handlers.TryGetValue(messageInfo.Opcode, out actions)) { // 系统中，不曾出现在过，不曾注册过相应操作码的消息处理器。一般出现在双端的某一端，客户端 for-example
                Log.Error($"消息没有处理: {messageInfo.Opcode} {JsonHelper.ToJson(messageInfo.Message)}");
                return;
            }
            foreach (IMHandler ev in actions) {
                try {
                    ev.Handle(session, messageInfo.Message); // 调用处理器的相应方法来处理消息 
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}