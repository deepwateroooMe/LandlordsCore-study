using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
namespace ETHotfix {

    [ObjectSystem]
    public class LandlordsLobbyComponentAwakeSystem : AwakeSystem<LandlordsLobbyComponent> {
        public override void Awake(LandlordsLobbyComponent self) {
            self.Awake();
        }
    }

    // 大厅界面组件
    public class LandlordsLobbyComponent : Component {
        public void Awake() {
            Init();
        }
        // 开始匹配按钮事件
        public async void OnStartMatch() { 
            try {
                // 发送开始匹配消息
                C2G_StartMatch_Req c2G_StartMatch_Req = new C2G_StartMatch_Req();
                G2C_StartMatch_Ack g2C_StartMatch_Ack = await SessionComponent.Instance.Session.Call(c2G_StartMatch_Req) as G2C_StartMatch_Ack; // 这里去看下服务器的处理逻辑
                if (g2C_StartMatch_Ack.Error == ErrorCode.ERR_UserMoneyLessError) {
                    Log.Error("余额不足"); // 就是说，当且仅当余额不足的时候才会出这个错误？
                    return;
                }
                // 匹配成功了：UI 界面切换，切换到房间界面
                UI room = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsRoom); // 装载新的UI视图
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.LandlordsLobby);          // 卸载旧的UI视图
                // 将房间设为匹配状态
                room.GetComponent<LandlordsRoomComponent>().Matching = true;
            }
            catch (Exception e) {
                Log.Error(e.ToStr());
            }
        }
        // 初始化: 视图里控件的点击回调事件的注册，逻辑编写等
        private async void Init() {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>(); // 这里为什么去拿父控件UI 上的 rc, 而不用它自己的 ？
            // 添加事件
            rc.Get<GameObject>("StartMatch").GetComponent<Button>().onClick.Add(OnStartMatch);
            // 获取玩家数据: 按说应该是注册登录服的逻辑，或者是数据库服存放着用户信息，都是通过Gate中转的吗？
            long userId = ClientComponent.Instance.LocalPlayer.UserID; // 当地玩家：是前一步，客户端登录成功的时候设置的
            C2G_GetUserInfo_Req c2G_GetUserInfo_Req = new C2G_GetUserInfo_Req() { UserID = userId }; // 去从网关服拿玩家信息
            G2C_GetUserInfo_Ack g2C_GetUserInfo_Ack = await SessionComponent.Instance.Session.Call(c2G_GetUserInfo_Req) as G2C_GetUserInfo_Ack;
            // 显示用户信息
            rc.Get<GameObject>("NickName").GetComponent<Text>().text = g2C_GetUserInfo_Ack.NickName;
            rc.Get<GameObject>("Money").GetComponent<Text>().text = g2C_GetUserInfo_Ack.Money.ToString();
        }
    }
}