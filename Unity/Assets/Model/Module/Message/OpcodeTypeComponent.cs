using System;
using System.Collections.Generic;
namespace ETModel {

 // Awake() Load()    
    [ObjectSystem]
    public class OpcodeTypeComponentSystem : AwakeSystem<OpcodeTypeComponent> {
        public override void Awake(OpcodeTypeComponent self) {
            self.Load();
        }
    }
    [ObjectSystem]
    public class OpcodeTypeComponentLoadSystem : LoadSystem<OpcodeTypeComponent> {
        public override void Load(OpcodeTypeComponent self) {
            self.Load();
        }
    }

    public class OpcodeTypeComponent : Component {
        private readonly DoubleMap<ushort, Type> opcodeTypes = new DoubleMap<ushort, Type>(); // 这里面封装了两个字典，所以从A取B，从B取A可以双通
        private readonly Dictionary<ushort, object> typeMessages = new Dictionary<ushort, object>();

// OpcodeTypeComponent加载封装好的网络消息。        
        public void Load() {
            this.opcodeTypes.Clear();
            this.typeMessages.Clear();
            
            List<Type> types = Game.EventSystem.GetTypes(typeof(MessageAttribute));
            foreach (Type type in types) {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attrs.Length == 0) {
                    continue;
                }
                MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                if (messageAttribute == null) {
                    continue;
                }
                this.opcodeTypes.Add(messageAttribute.Opcode, type);
                this.typeMessages.Add(messageAttribute.Opcode, Activator.CreateInstance(type)); // 程序集？第一次加载的时候，不得不创建新的实例
            }
        }
        public ushort GetOpcode(Type type) {
            return this.opcodeTypes.GetKeyByValue(type);
        }
        public Type GetType(ushort opcode) {
            return this.opcodeTypes.GetValueByKey(opcode); // 拿的是倒了序的字典里的值，是原顺序字典里的键
        }

        // 客户端为了0  GC需要消息池，服务端消息需要跨协程不需要消息池：这里还是没有读明白
        public object GetInstance(ushort opcode) {
#if SERVER // 总之，是服务器，就不需要什么消息池
            Type type = this.GetType(opcode);
            return Activator.CreateInstance(type);
#else // 其它的，就是需要消息池的  ？
            return this.typeMessages[opcode]; // 这里取现的
#endif
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
    }
}