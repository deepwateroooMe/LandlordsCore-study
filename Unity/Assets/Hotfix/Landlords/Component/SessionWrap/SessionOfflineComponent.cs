using ETModel;
namespace ETHotfix {

    // 用于Session断开时触发下线: 就是说，客户端与网关服的会话消息若是断开，便自动触发下线
    public class SessionOfflineComponent : Component {

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            // 移除连接组件
            Game.Scene.RemoveComponent<SessionComponent>();
            Game.Scene.ModelScene.RemoveComponent<ETModel.SessionComponent>();
            // 释放本地玩家对象
            ClientComponent clientComponent = ETModel.Game.Scene.GetComponent<ClientComponent>();
            if (clientComponent.LocalPlayer != null) {
                clientComponent.LocalPlayer.Dispose();
                clientComponent.LocalPlayer = null;
            }
            UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            // 游戏关闭，不用回到登录界面
            if(uiComponent == null || uiComponent.IsDisposed) {
                return;
            }
            UI uiLogin = uiComponent.Create(UIType.LandlordsLogin); // 这里是真的，又创建了一个新的？希望它能够拿到现有的销毁前的，要不然太浪费性能了。要自己确认一下这里
            uiLogin.GetComponent<LandlordsLoginComponent>().SetPrompt("连接断开");
            if (uiComponent.Get(UIType.LandlordsLobby) != null) {
                uiComponent.Remove(UIType.LandlordsLobby);
            } else if (uiComponent.Get(UIType.LandlordsRoom) != null) {
                uiComponent.Remove(UIType.LandlordsRoom);
            }
        }
    }
}
