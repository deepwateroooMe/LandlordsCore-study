using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ETModel {
    public abstract class NetworkComponent : Component {

        public AppType AppType;
        protected AService Service;

        // 这个组件管理不上一个session, 所以要管理一下
        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

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
                    this.Service = new TService(packetSize, ipEndPoint, this.OnAccept);
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
        public virtual void Remove(long id) { // 移除掉某个过期,或是用户登出去了的session
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