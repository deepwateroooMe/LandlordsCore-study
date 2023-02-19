 using ETModel;
using System.Net;
namespace ETHotfix {
    public static class MapHelper {

        // 发送消息给【匹配服务器】
        public static void SendMessage(IMessage message) {
            GetMapSession().Send(message);
        }
        // 获取匹配服务器连接
        public static Session GetMapSession() {
            IPEndPoint matchIPEndPoint = Game.Scene.GetComponent<StartConfigComponent>().MatchConfig.GetComponent<InnerConfig>().IPEndPoint;
            Session matchSession = Game.Scene.GetComponent<NetInnerComponent>().Get(matchIPEndPoint);
            return matchSession;
        }
    }
}
