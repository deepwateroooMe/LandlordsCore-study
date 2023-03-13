using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
namespace ETModel {

    [ObjectSystem]
    public class SessionAwakeSystem : AwakeSystem<Session, AChannel> {
        public override void Awake(Session self, AChannel b) {
            self.Awake(b);
        }
    }

 // 这里没有弄懂：
    
    // 服务器响应一个反馈消息。 定义消息或者rpc请求的时候使用Message标签设置消息类型与opcode的对应关系。 session是由NetworkComponent创建的，参数是 NetworkComponent的本类实例 + 用于收发字节码的AChannel实例。这个AChannel则是由NetworkComponent中的AService工厂创建的。
    // 流程：（自己之前不是写过 WebSocket 双端通讯的小例子吗？就是把原本在信道上 Channel 上的三个异步操作：接收，读，和读完？封装到了 session 里而已呀）
    // 1创建session后（调用构造函数），session开始StartRecv()方法，这个方法循环通过异步方法调用channel的Recv()方法，接收链接另一端发来的数据。（这里说的应该是服务器端）
    // 2接收到消息后调用Run方法，run方法检查接收到的数据包是否有压缩，并指向相应的操作。（这里感觉跟源码稍微有点儿不一样）
    // 3处理完压缩解压操作后交给RunDecompressedBytes，该方法比较厉害，调用绑定在Scene实体上的NetWorkConponent上的注册的解包器（默认是Bson）解包。
    // 4解包操作结束后就将其交给绑定在Scene实体上的NetWorkConponent上的messageDispatcher做转发。转发给相应的handler处理。

    public sealed class Session : Entity { // 这个类，是网络异步调用的会话框：想要多看几遍，是异步网络调用中的一个梳杻

        private static int RpcId { get; set; } // 静态
        private AChannel channel;

        // Rpc调用回调的字典管理：值是Rpc回调回IResponse后的回调函数
        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();
        private readonly List<byte[]> byteses = new List<byte[]>() { new byte[1], new byte[2] }; // byteses应该就是通道中传输的数据了

        public NetworkComponent Network { // NetInnerComponent || NetOuterComponent
            get { // Session 的 Parent 是：ETModel.Game.Scene 吗？
                // 如果这是个客户端，它 Session是在哪里创建的？ onLogin() 的时候，添加在 Game.Scene.ModelScene+SceneComponent
                // 把父控件作为NetworkComponent返回，拿的是抽象基类，拿到的会是 NetInnerComponent||NetOuterComponent, 那么它的父类就应该是，ETModel.Game.Scene
                // 在后面消息发送的时候，分内网消息，外网消息封装，就可以再看見这一层抽象包装可以起到的分类作用
                return this.GetParent<NetworkComponent>();  // Component.GetParent() 是Component 及其子类就可以拿到其父控件
            }
        }
        public int Error {
            get {
                return this.channel.Error; // 返回的是，通信信道上抛出的异常
            }
            set {
                this.channel.Error = value;
            }
        }
        public void Awake(AChannel aChannel) {
            this.channel = aChannel;
            this.requestCallback.Clear();
            long id = this.Id;
            channel.ErrorCallback += (c, e) => {
                this.Network.Remove(id); 
            };
            channel.ReadCallback += this.OnRead; // 初始化（有意识清醒时），注册必要的相关的回调：有数据可读，就调用这里的函数来读
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            long id = this.Id;
            base.Dispose();
 // 这里抛出的全部都是出错回调：是因为,在缓存有回调，但是都还没有能够触发时，就被要求回收了，所以不得不通知那些等待接收回调的被通知者，抱歉，出错了。。。。。            
            foreach (Action<IResponse> action in this.requestCallback.Values.ToArray()) {
                action.Invoke(new ResponseMessage { Error = this.Error });
            }
            // int error = this.channel.Error;
            // if (this.channel.Error != 0)
            // {
            //    Log.Trace($"session dispose: {this.Id} ErrorCode: {error}, please see ErrorCode.cs!");
            // }
            
            this.channel.Dispose(); // 必须得把这些个所有的持有与引用全部清空，才能够真正的释放资源，否则memory leak ?  我们C＃程序员已经不屑于再提这此小细节了。。。。。
            this.Network.Remove(id); // Session ＝＝》 Entity ＝＝》 ComponentWithId ＝＝》 Component 它是有ID 的，就是移除当前组件
            this.requestCallback.Clear(); // 
        }

        public void Start() {
            this.channel.Start(); // 就是去处理信道的异步连接，异步接受，异步读取等等
        }
        public IPEndPoint RemoteAddress {
            get {
                return this.channel.RemoteAddress;
            }
        }
        public ChannelType ChannelType { // 是指方向流向
            get {
                return this.channel.ChannelType;
            }
        }
        public MemoryStream Stream {
            get {
                return this.channel.Stream;
            }
        }

        public void OnRead(MemoryStream memoryStream) { // 这就是2: 接到消息后调用 Run() 方法呀
            try {
                this.Run(memoryStream);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        private void Run(MemoryStream memoryStream) { // 这就是从内存上发消息
            memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin); // 快进到，消息头
            byte flag = memoryStream.GetBuffer()[Packet.FlagIndex];

            // BitConverter.ToUInt16 这个方法是将字节数组指定位置起的两个字节转换为无符号整数。所以我们得先保证messageBytes的长度是大于等于3的。
            // 这里消息传递的结构：2个字节的操作类型+具体消息。
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.OpcodeIndex);
            
#if !SERVER
            if (OpcodeHelper.IsClientHotfixMessage(opcode)) { // 如果是，来自于客户端的热更新消息
                 // 它说如果是来自于客户端的热更新消息，并且热更层注册了索要回馈通知，就要触发相关的回调。
                // 会话框消息，返回不返回，不一定，可能是这种情况下特有，于是它就把回调再重新包装成组件，实现组件的随需要装卸
                this.GetComponent<SessionCallbackComponent>().MessageCallback.Invoke(this, flag, opcode, memoryStream); // <<<<<<<<<< 回调的逻辑定义在哪里呢？
                return; 
            }
#endif
            object message;
            try {
                // 这个组件 OpcodeTypeComponent, 它已经早就实例化了各种操作符各有一个实例备用。还没想明白，这是什么奴役（各种客户端会用到的外网渻消息的实例）
                OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
                
                object instance = opcodeTypeComponent.GetInstance(opcode); // 特定类型的消息实例
                // 这就是，内存流上读消息了，速度应该是很快。主要是如果是服务器间内网消息的话，就直接转发，不用再反序列化，又再序列化才发送了。它说这样客户端不能存消息来减少GC 来着？
                message = this.Network.MessagePacker.DeserializeFrom(instance, memoryStream); // 利用消息封装体里的反序列化工具将内存流中的消息反序列化到消息实例里去                
                if (OpcodeHelper.IsNeedDebugLogMessage(opcode)) {
                    Log.Msg(message);
                }
            }
            catch (Exception e) {
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                Log.Error($"opcode: {opcode} {this.Network.Count} {e} ");
                this.Error = ErrorCode.ERR_PacketParserError;
                this.Network.Remove(this.Id); // 把这个会话移除
                return;
            }
            // flag第一位为1表示这是rpc返回消息; 否则交由MessageDispatcher分发消息
            
            if ((flag & 0x01) == 0) {
                this.Network.MessageDispatcher.Dispatch(this, opcode, message); // 意思是说：这个分支，是内网消息，需要分发？
                return;
            }
            IResponse response = message as IResponse; // 这里是，处理回复消息的逻辑
            if (response == null) {
                throw new Exception($"flag is response, but message is not! {opcode}");
            }
            Action<IResponse> action;
            if (!this.requestCallback.TryGetValue(response.RpcId, out action)) {
                return;
            }
 // 如果这个response.RpcId 被消息的发送方注册了相关的消息接收回调，就先从管理字典中，先移除回调，再触发回调一次            
            this.requestCallback.Remove(response.RpcId);
            action(response);
        }

// 向session另一端做rpc调用可以调用它的Call方法，打电话是需要对方接听才能够连得通的，这个方法需要返回一条回复IResponse
// 发送消息可以调用Send， 二者的区别在于是否需要服务器响应一个反馈消息。
        public Task<IResponse> Call(IRequest request) { // 这个应该是需要服务器响应一个反馈信息的 ？
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>(); // 封装一个异步任务，需要返回消息给调用者
            this.requestCallback[rpcId] = (response) => {    // 字典里键rpcId所对应的值，具体的回调函数就如  入  定义
                try {
                    if (ErrorCode.IsRpcNeedThrowException(response.Error)) { // 出错了就抛异常
                        throw new RpcException(response.Error, response.Message);
                    }
                    tcs.SetResult(response); // 回调逻辑：没出错，就写好异步任务的结果，也就是异步返回结果了呀。异步任务的结果写好了，是不是这个调用就结束了？
                }
                catch (Exception e) {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };
            request.RpcId = rpcId;
            this.Send(0x00, request); // 把这个请求消息发出去
            return tcs.Task; // 这里是，返回异步网络调用的结果
        }
        public Task<IResponse> Call(IRequest request, CancellationToken cancellationToken) {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();
            this.requestCallback[rpcId] = (response) => {
                try {
                    if (ErrorCode.IsRpcNeedThrowException(response.Error))
                    {
                        throw new RpcException(response.Error, response.Message);
                    }
                    tcs.SetResult(response);
                }
                catch (Exception e) {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
                }
            };
            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));
            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

// 发送消息可以调用Send， 二者的区别在于是否需要服务器响应一个反馈消息。Send发出去的，是可以不要返回消息的，像发短讯一样，发出去就完了
        public void Send(IMessage message) {
            this.Send(0x00, message);
        }
        public void Reply(IResponse message) {
            if (this.IsDisposed) {
                throw new Exception("session已经被Dispose了");
            }
            this.Send(0x01, message);
        }
        public void Send(byte flag, IMessage message) {
// 想一下这里有几个的问题： ETModel 中 OpcodeTypeComponent 应该是有且仅有一个，里面各种不同操作符的会话框各有一个实例；每个客户端持有一个与网关服 Gate 的会话框 Session
            OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>(); // 这个是客户端的吗？找一下哪里加载的这个组件？
            // 每个消息分发组件都会持有一个 OpcodeTypeComponent. 就是上面那句话写得有点儿怪异 this.Network.Entity，有点儿连不起来。。。。。
            ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());
            
            Send(flag, opcode, message);
        }
        public void Send(byte flag, ushort opcode, object message) {
            if (this.IsDisposed) {
                throw new Exception("session已经被Dispose了");
            }
            
            if (OpcodeHelper.IsNeedDebugLogMessage(opcode) ) {
#if !SERVER
                if (OpcodeHelper.IsClientHotfixMessage(opcode)) {
                } else
#endif 
                {
                    Log.Msg(message);
                }
            }
// ETHotfix 层的消息，还是会走到这里 ETModel Session 这里来真正从内存流上发出去：
// 这里，理论上的理解是，ETHotfix, ETModel, 更多的是我们程序员框架架构上的分层分域，用来区分可热重载与不可热重载，但是底层，消息的发送，可能就是瞬间的事，一个逻辑的分支而已            
            MemoryStream stream = this.Stream;
            stream.Seek(Packet.MessageIndex, SeekOrigin.Begin); // 快进到指定的位置
            stream.SetLength(Packet.MessageIndex); // 应该是：必须设定消息长度
             // 具体的包装体实现类，可以是Mongo,可以是Protobuf. 这个框架，将外网消息全部设为 ProtobufPacker, 将内网消息全部设为 MongoPacker
            this.Network.MessagePacker.SerializeTo(message, stream); // 这里是上次没能连接起来的地方：就是（跨进程？）异步网络调用的消息体的序列化，它是封装到了消息的包装体类里，唯客户端需要这个消息包装（？对吗，这里不是说是序列化吗，序列化是凡跨进程就需要的，服务器间跨进程同样需要？好像这里还有点儿概念上的混淆）  ？
            stream.Seek(0, SeekOrigin.Begin); // 把消息成功序列化到内存流中去了，仍将内存流的头重置回头上
// 这里，上面是，先把消息序列化进内存流消息内容应该呆的位置；接下来部分，是把消息头重新在字节数组中写好准备好，再高效复制到内存流中头上的位置（这个头需要序列化）
            this.byteses[0][0] = flag;
            this.byteses[1].WriteTo(0, opcode);
            int index = 0;
            foreach (var bytes in this.byteses) { // 以每个数组为单位，遍历链表。复制完了，内存流的头仍在0 位置 
                Array.Copy(bytes, 0, stream.GetBuffer(), index, bytes.Length); // 这里是，从本地字节数组缓存链表，复制到了内存流中
                index += bytes.Length;
            }
#if SERVER
            // 如果是allserver，内部消息不走网络，直接转给session,方便调试时看到整体堆栈
            if (this.Network.AppType == AppType.AllServer) {
                Session session = this.Network.Entity.GetComponent<NetInnerComponent>().Get(this.RemoteAddress);
                session.Run(stream);
                return;
            }
#endif
            this.Send(stream); // 内存流：到这里，就实现了内存流上发消息。但是使用内存流，内网消息优化的关节，应该是在网关服接收到消息，不反序列化，直接根据消息类型转发的地方
        }
        public void Send(MemoryStream stream) {
            channel.Send(stream);
        }
    }
}