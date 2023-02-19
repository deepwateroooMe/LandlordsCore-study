using System.Collections.Generic;
using System.Net;
namespace ETModel {
    public class NetInnerComponent: NetworkComponent { // 内网消息：是服务器之间的
        // 本地缓存：与其它每个需要连接的服务器都缓存一个 Session, 用于与这个服务器通信
        public readonly Dictionary<IPEndPoint, Session> adressSessions = new Dictionary<IPEndPoint, Session>();

        public override void Remove(long id) {
            Session session = this.Get(id);
            if (session == null) {
                return;
            }
            this.adressSessions.Remove(session.RemoteAddress);
            base.Remove(id);
        }

        // 从地址缓存中取Session,如果没有则创建一个新的Session,并且保存到地址缓存中,它就是封装好了同每个端（服务器）会话的Session，与每个端有唯一一个session
        // 任何服务器都有唯一IP 地址标识，与任何服务器端通信，都需要一个 Session; 单线程多进程的架构，决定了任何时候与任何某个服务器都是单线程逻辑，也就是与那个端只有一个Session
        public Session Get(IPEndPoint ipEndPoint) {
            if (this.adressSessions.TryGetValue(ipEndPoint, out Session session)) {
                return session;
            }
            session = this.Create(ipEndPoint);
            this.adressSessions.Add(ipEndPoint, session);
            return session;
        }
    }
}