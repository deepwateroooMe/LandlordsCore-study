using System.Net;

namespace ETModel {

    // 知道对方的instanceId，使用这个类发actor消息: 为什么是对方的instanceId?是instanceId一定可发吗？下线了呢，过期了呢？
    public struct ActorMessageSender {

        // actor的地址：【自己加，这里应该是，对方的，接收方的 IP 地址】
        public IPEndPoint Address { get; } // 就是说要给要给亲爱的表哥发消息，必须得知道亲爱的表哥的电话号码，是必须得知道对方的，不是自己的
        public long ActorId { get; }       // 同样是，对方的 ActorId
        // 上面的两上，可能某些情况下，可以某个不要？既然 ActorId 带有终端地址信息，不是可以只要一个参数就可以了吗？可能也有某种惯例 ?
        public ActorMessageSender(long actorId, IPEndPoint address) { // 主要是不有弄清楚，这两个参数，哪个是发送方还是接收方？
            this.ActorId = actorId;
            this.Address = address;
        }
    }
}