using ETModel;
namespace ETHotfix {
    // 用于Session断开时触发下线: 就是说，客户端与网关服的会话消息若是断开，便自动触发下线。这是斗地主游戏里添加的组件
    public class SessionOfflineComponent : Component {
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();

            // 移除异步网络连接，相关的组件: Session 掉线，就是说 Session 需要被回收呀 
            Game.Scene.RemoveComponent<SessionComponent>(); // 这里可能是热重载域的？
            Game.Scene.ModelScene.RemoveComponent<ETModel.SessionComponent>(); // 这里是固定不可热更新层的？这两个还有点儿混

            // 移除客户端，相关的组件：            
            // 释放 ETModel 场景里，本地玩家，客户端对象：在ETModel 启动的时候会添加这个客户端组件
            ClientComponent clientComponent = ETModel.Game.Scene.GetComponent<ClientComponent>();
            if (clientComponent.LocalPlayer != null) {
                clientComponent.LocalPlayer.Dispose();
                clientComponent.LocalPlayer = null;
            }
            UIComponent uiComponent = Game.Scene.GetComponent<UIComponent>();
            // 游戏关闭，不用回到登录界面
            if (uiComponent == null || uiComponent.IsDisposed) {
                return;
            }
            UI uiLogin = uiComponent.Create(UIType.LandlordsLogin);
            uiLogin.GetComponent<LandlordsLoginComponent>().SetPrompt("连接断开");
            if (uiComponent.Get(UIType.LandlordsLobby) != null) {
                uiComponent.Remove(UIType.LandlordsLobby);
            } else if (uiComponent.Get(UIType.LandlordsRoom) != null) {
                uiComponent.Remove(UIType.LandlordsRoom);
            }
        }
    }
}
