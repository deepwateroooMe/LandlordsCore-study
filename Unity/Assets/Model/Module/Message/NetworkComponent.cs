using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ETModel {

    // 这个类是双端共享源码的类：双端都可以用,但某此成员可能只是提供给某一端来使用的，比如客户端的消息打包器与消息转发器
    // 抽象了udp和tcp协议的连接工厂，用于创建channel使用tcp做服端的时候可以创建多个Session，每个Session都是一个连接的高层抽象。
    public abstract class NetworkComponent : Component {

        public AppType AppType;
        protected AService Service;

        // 这个组件管理不上一个session, 所以要管理一下
        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        // 这个类是双端共享源码的类：双端都可以用,但某此成员可能只是提供给某一端来使用的，比如客户端的消息打包器与消息转发器
        // 其提供的MessageDispatch和MessagePacker仅在客户端项目使用。一个是客户端消息转发器，一个是客户端消息打包器。
        public IMessagePacker MessagePacker { get; set; }
        public IMessageDispatcher MessageDispatcher { get; set; }

         // 这里也就说明,一个NetworkComponent实例,只可能是一种某种特定的网络规范服务
        public void Awake(NetworkProtocol protocol, int packetSize = Packet.PacketSizeLength2) {
            switch (protocol) {
            case NetworkProtocol.KCP:
                this.Service = new KService();
                break;
            case NetworkProtocol.TCP:
                this.Service = new TService(packetSize);
                break;
            case NetworkProtocol.WebSocket:
                this.Service = new WService();
                break;
            }
        }
        public void Awake(NetworkProtocol protocol, string address, int packetSize = Packet.PacketSizeLength2) {
            try {
                IPEndPoint ipEndPoint;
                switch (protocol) {
                case NetworkProtocol.KCP:
                    ipEndPoint = NetworkHelper.ToIPEndPoint(address);
                    this.Service = new KService(ipEndPoint, this.OnAccept);
                    break;
                case NetworkProtocol.TCP:
                    ipEndPoint = NetworkHelper.ToIPEndPoint(address);
                    this.Service = new TService(packetSize, ipEndPoint, this.OnAccept); // <<<<<<<<<<<<<<<<<<<< 这里就注册了这个异步接收到数据的回调
                    break;
                case NetworkProtocol.WebSocket:
                    string[] prefixs = address.Split(';');
                    this.Service = new WService(prefixs, this.OnAccept);
                    break;
                }
            }
            catch (Exception e) {
                throw new Exception($"NetworkComponent Awake Error {address}", e);
            }
        }
        public int Count {
            get { return this.sessions.Count; }
        }

        public void OnAccept(AChannel channel) {
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session); // Component组件是带id的
            session.Start(); // 这里应该是一个异步开始接收连接的方法
        }
        public virtual void Remove(long id) { // 移除掉某个过期,或是用户登出去了的,  或是出错了的  session
            Session session;
            if (!this.sessions.TryGetValue(id, out session)) {
                return;
            }
            this.sessions.Remove(id);
            session.Dispose();
        }
        public Session Get(long id) {
            Session session;
            this.sessions.TryGetValue(id, out session);
            return session;
        }

        // 创建一个新Session
        public Session Create(IPEndPoint ipEndPoint) {
            AChannel channel = this.Service.ConnectChannel(ipEndPoint); // 都是通过服务,来创建信道的
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session);
            session.Start();
            return session;
        }
        // 创建一个新Session
        public Session Create(string address) {
            AChannel channel = this.Service.ConnectChannel(address);
            Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
            this.sessions.Add(session.Id, session);
            session.Start();
            return session;
        }

        public void Update() {
            if (this.Service == null) {
                return;
            }
            this.Service.Update();
        }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            foreach (Session session in this.sessions.Values.ToArray()) { // 转化为数组,是因为顺序遍历更快一点儿吗?
                session.Dispose();
            }
            this.Service.Dispose();
        }
    }
}