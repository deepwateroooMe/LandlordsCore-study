using System.Collections.Generic;
using System.Net;
namespace ETModel {
    public class NetInnerComponent: NetworkComponent { // 内网组件：管理内网各服务器间的连接地址信息会话框实例等

        // 这里有点儿忘记了：这个字典是在什么时候初始化填充，与各不同内网服务器之间的会话框实例的？【可能是根据配置文件来加载的时候，要再找一下】
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