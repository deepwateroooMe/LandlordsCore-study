using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.IO;

namespace ETModel {

    // 封装Socket,将回调push到主线程处理: 这个注释还需要再理解一下,为什么需要推到主线程?单线程逻辑?
    public sealed class TChannel: AChannel {

        private Socket socket;
 // 没有细读这里面的成员，需要找个讲解文参考一下，这里跳过
        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();
        private readonly MemoryStream memoryStream;

        // 因为都是异步,这需要这引起标记来帮助表明,异步是否执行完成
        private bool isSending;
        private bool isRecving;
        private bool isConnected;

        private readonly PacketParser parser;
        private readonly byte[] packetSizeCache;
        
        public TChannel(IPEndPoint ipEndPoint, TService service): base(service, ChannelType.Connect) {
            int packetSize = this.GetService().PacketSizeLength;
            this.packetSizeCache = new byte[packetSize];
            this.memoryStream = this.GetService().MemoryStreamManager.GetStream("message", ushort.MaxValue);
            
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;
            this.parser = new PacketParser(packetSize, this.recvBuffer, this.memoryStream);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;
            this.RemoteAddress = ipEndPoint;
            this.isConnected = false;
            this.isSending = false;
        }
        public TChannel(Socket socket, TService service): base(service, ChannelType.Accept) {
            int packetSize = this.GetService().PacketSizeLength;
            this.packetSizeCache = new byte[packetSize];
            this.memoryStream = this.GetService().MemoryStreamManager.GetStream("message", ushort.MaxValue);
            
            this.socket = socket;
            this.socket.NoDelay = true;
            this.parser = new PacketParser(packetSize, this.recvBuffer, this.memoryStream);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;
            this.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;
            
            this.isConnected = true;
            this.isSending = false;
        }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose(); // 都是处理完基类所有共通的逻辑,再处理继承附加逻辑
            this.socket.Close();
            this.innArgs.Dispose();
            this.outArgs.Dispose();
            this.innArgs = null;
            this.outArgs = null;
            this.socket = null;
            this.memoryStream.Dispose();
        }
        
        private TService GetService() {
            return (TService)this.Service;
        }
        public override MemoryStream Stream {
            get {
                return this.memoryStream;
            }
        }

 // 这个过程总体是: ConnectAsync()端之间异步连接， StartRecv()开始异步接受数据， 
        public override void Start() {
            if (!this.isConnected) {
                this.ConnectAsync(this.RemoteAddress);
                return;
            }
            if (!this.isRecving) {
                this.isRecving = true;
                this.StartRecv();
            }
            this.GetService().MarkNeedStartSend(this.Id); // 会在this.GetService()的Update()中去StartSend()异步发送
        }
        
        public override void Send(MemoryStream stream) {
            if (this.IsDisposed) {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            switch (this.GetService().PacketSizeLength) {
                case Packet.PacketSizeLength4:
                    if (stream.Length > ushort.MaxValue * 16)
                    {
                        throw new Exception($"send packet too large: {stream.Length}");
                    }
                    this.packetSizeCache.WriteTo(0, (int) stream.Length);
                    break;
                case Packet.PacketSizeLength2:
                    if (stream.Length > ushort.MaxValue)
                    {
                        throw new Exception($"send packet too large: {stream.Length}");
                    }
                    this.packetSizeCache.WriteTo(0, (ushort) stream.Length);
                    break;
                default:
                    throw new Exception("packet size must be 2 or 4!");
            }
            this.sendBuffer.Write(this.packetSizeCache, 0, this.packetSizeCache.Length);
            this.sendBuffer.Write(stream);
            this.GetService().MarkNeedStartSend(this.Id);
        }

        private void OnComplete(object sender, SocketAsyncEventArgs e) {
            switch (e.LastOperation) {
                case SocketAsyncOperation.Connect:
                    OneThreadSynchronizationContext.Instance.Post(this.OnConnectComplete, e);
                    break;
                case SocketAsyncOperation.Receive:
                    OneThreadSynchronizationContext.Instance.Post(this.OnRecvComplete, e);
                    break;
                case SocketAsyncOperation.Send:
                    OneThreadSynchronizationContext.Instance.Post(this.OnSendComplete, e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    OneThreadSynchronizationContext.Instance.Post(this.OnDisconnectComplete, e);
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }
        public void ConnectAsync(IPEndPoint ipEndPoint) {
            this.outArgs.RemoteEndPoint = ipEndPoint;
            if (this.socket.ConnectAsync(this.outArgs)) { // 正在连接进行中，请稍候。。。
                return;
            }
            OnConnectComplete(this.outArgs); // 连接完成，触发连接完成回调
        }
        private void OnConnectComplete(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;
            
            if (e.SocketError != SocketError.Success) { // 连接失败，触发失败回调
                this.OnError((int)e.SocketError);    
                return;
            }
            e.RemoteEndPoint = null;
            this.isConnected = true; // 更新状态
            this.Start();
        }
        private void OnDisconnectComplete(object o) {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            this.OnError((int)e.SocketError);
        }
 // 开始异步读取数据：以常量大小为块，来以块块块来读㻓数据
        private void StartRecv() {
            int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex; // 当前块，能够读取的最大大小，＜= 8K 8192 bytes
            this.RecvAsync(this.recvBuffer.Last, this.recvBuffer.LastIndex, size); // 异步读取接收：读到下标为this.recvBuffer.Last的接收块，从块的LastIndex下标写起，写size大小
        }
        public void RecvAsync(byte[] buffer, int offset, int count) {
            try {
                this.innArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e) {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
            
            if (this.socket.ReceiveAsync(this.innArgs)) {
                return;
            }
            OnRecvComplete(this.innArgs);
        }
        private void OnRecvComplete(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;
            if (e.SocketError != SocketError.Success) {
                this.OnError((int)e.SocketError);
                return;
            }
            if (e.BytesTransferred == 0) {
                this.OnError(ErrorCode.ERR_PeerDisconnect);
                return;
            }
            this.recvBuffer.LastIndex += e.BytesTransferred; // 从写缓冲区上次写尾巴接着往写
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize) {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }
            // 收到消息回调
            while (true) {
                try {
                    if (!this.parser.Parse())
                    {
                        break;
                    }
                }
                catch (Exception ee) {
                    Log.Error(ee);
                    this.OnError(ErrorCode.ERR_SocketError);
                    return;
                }
                try {
                    this.OnRead(this.parser.GetPacket()); // 那边写完了一个缓冲区大小（写到了哪里？内存流？），这边就触发了读回调，开始以缓冲区大小为单位来读取（内存流？） 
                }
                catch (Exception ee) {
                    Log.Error(ee);
                }
            }
            if (this.socket == null) {
                return;
            }
            this.StartRecv(); // 这个细节点儿，还有点儿没懂，再看一下
        }

 // 异步发送相关的回调：        
        public bool IsSending => this.isSending;
        public void StartSend() {
            if(!this.isConnected) {
                return;
            }
            // 没有数据需要发送
            if (this.sendBuffer.Length == 0) {
                this.isSending = false;
                return;
            }
            this.isSending = true;
            int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
            if (sendSize > this.sendBuffer.Length) { // 狠怪异：什么情况下会走到这个分支来呢？
                sendSize = (int)this.sendBuffer.Length;
            }
            this.SendAsync(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
        }
        public void SendAsync(byte[] buffer, int offset, int count) {
            try {
                this.outArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e) {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
            if (this.socket.SendAsync(this.outArgs)) {
                return;
            }
            OnSendComplete(this.outArgs);
        }
        private void OnSendComplete(object o) {
            if (this.socket == null) {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;
            if (e.SocketError != SocketError.Success) {
                this.OnError((int)e.SocketError);
                return;
            }
            if (e.BytesTransferred == 0) {
                this.OnError(ErrorCode.ERR_PeerDisconnect);
                return;
            }
            this.sendBuffer.FirstIndex += e.BytesTransferred; // 更新到真正需要起始发送的地方
            if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize) { // 如果当前块读完了，把这个块移走回收，从下一个块开始读，更新读下标
                this.sendBuffer.FirstIndex = 0;
                this.sendBuffer.RemoveFirst();
            }
            this.StartSend();
        }
    }
}