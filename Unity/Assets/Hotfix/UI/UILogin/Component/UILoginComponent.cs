using System;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
namespace ETHotfix {
    [ObjectSystem]
    public class UiLoginComponentSystem : AwakeSystem<UILoginComponent> {
        public override void Awake(UILoginComponent self) {
            self.Awake();
        }
    }
    public class UILoginComponent: Component {
        private GameObject account;
        private GameObject loginBtn;
        public void Awake() {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            loginBtn = rc.Get<GameObject>("LoginBtn");
            loginBtn.GetComponent<Button>().onClick.Add(OnLogin);
            this.account = rc.Get<GameObject>("Account");
        }
        public async void OnLogin() { // 这就是登录的回調了
            try {
                string text = this.account.GetComponent<InputField>().text;

// 这里的层次概念就出来：热更新层 ETHotfix 有一个 Session; 不可热更新层 ETModel 也有一个Session;
                // 创建一个ETModel层的Session: 因为是客户端要发消息给注册登录服，所以是外网消息
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);
                // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息，这里就更顶层一点儿
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session); // 这里再创建一遍，有个ID 要赋值，仍是拿上面 session 来创建的

                // 客户端调用 realm 登录服，等待返回 R2C_Login 消息
                R2C_Login r2CLogin = (R2C_Login) await realmSession.Call(new C2R_Login() { Account = text, Password = "111111" }); // <<<<<<<<<< Key
                realmSession.Dispose();  // 异步完成之后，把不用的会话回收掉，再往最底层去，就仍然是必要的时候回收资源池。就是用时包装，不用时释放

                // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中；前面ETMode 域的Init.cs 添加这个组件 
                ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address); 
                ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;
                // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
                Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
                
                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
                Log.Info("登陆gate成功!");
// 我的游戏的逻辑：可以只到这里，能够成功拿到网关服的 key, 就充当可以直接进入臫游戏热重载的许可，进入游戏。以后可以考虑通过网关服向数据库保存用戶游戏数据？                
// 客户端的逻辑大致清楚，也去找一下注册的过程？然后去看服务器端相关的逻辑
                
                // 创建Player
                Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
                PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
                playerComponent.MyPlayer = player;
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UILobby);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
                // 测试消息有成员是class类型
                G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
