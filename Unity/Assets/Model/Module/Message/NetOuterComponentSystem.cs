using ETModel;
namespace ETHotfix {

// Awake() Awake1() Load() Update()
// Awake?() Load(): 会实例化消息分发器
    [ObjectSystem]
    public class NetOuterComponentAwakeSystem : AwakeSystem<NetOuterComponent> {
        public override void Awake(NetOuterComponent self) {
            self.Awake(self.Protocol);
            self.MessagePacker = new ProtobufPacker(); // 外网消息：全都是用 ProtobufPacker 系统
            self.MessageDispatcher = new OuterMessageDispatcher(); 
        }
    }
    [ObjectSystem]
    public class NetOuterComponentAwake1System : AwakeSystem<NetOuterComponent, string> {
        public override void Awake(NetOuterComponent self, string address) {
            self.Awake(self.Protocol, address);
            self.MessagePacker = new ProtobufPacker();
            self.MessageDispatcher = new OuterMessageDispatcher(); 
        }
    }
    [ObjectSystem]
    public class NetOuterComponentLoadSystem : LoadSystem<NetOuterComponent> {
        public override void Load(NetOuterComponent self) {
            self.MessagePacker = new ProtobufPacker();
            self.MessageDispatcher = new OuterMessageDispatcher(); 
        }
    }
    
    [ObjectSystem]
    public class NetOuterComponentUpdateSystem : UpdateSystem<NetOuterComponent> {
        public override void Update(NetOuterComponent self) {
            self.Update();
        }
    }
}