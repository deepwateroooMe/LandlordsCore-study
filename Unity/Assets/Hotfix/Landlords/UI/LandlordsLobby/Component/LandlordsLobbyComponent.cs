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
        // 开始StartEducationalMode按钮事件：开始了真正的游戏逻辑
        // public async void OnStartMatch() {  // 点击事件回调的方法名，也是需要改的：EducationalMode async: 如果不是网络调用，还需要异步吗？
        public void OnStartMatch() {  // 点击事件回调的方法名，也是需要改的：EducationalMode async: 如果不是网络调用，还需要异步吗？
             // 要在这里面写视图的切换吗？再去找逻辑
            // UI room = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsRoom); // 装载新的UI视图，不需要进到这里面来
            UI uiEducationalMode = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIEducationalMode); // 进入到三种选择里去
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.LandlordsLobby);          // 卸载旧的UI视图

            // try {
            //     // 发送开始匹配消息：斗地主的游戏逻辑
            //     C2G_StartMatch_Req c2G_StartMatch_Req = new C2G_StartMatch_Req();
            //     G2C_StartMatch_Ack g2C_StartMatch_Ack = await SessionComponent.Instance.Session.Call(c2G_StartMatch_Req) as G2C_StartMatch_Ack;
            //     if (g2C_StartMatch_Ack.Error == ErrorCode.ERR_UserMoneyLessError) {
            //         Log.Error("余额不足"); // 就是说，当且仅当余额不足的时候才会出这个错误？
            //         return;
            //     }
            //     // 匹配成功了：切换到房间界面
            //     UI room = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsRoom); // 装载新的UI视图
            //     Game.Scene.GetComponent<UIComponent>().Remove(UIType.LandlordsLobby);          // 卸载旧的UI视图
            //     // 将房间设为匹配状态
            //     room.GetComponent<LandlordsRoomComponent>().Matching = true;
            // }
            // catch (Exception e) {
            //     Log.Error(e.ToStr());
            // }
        }
        // 初始化: 视图里控件的点击回调事件的注册，逻辑编写等
        private async void Init() { // 这些回调：都是体会异步调用，写成流式语法的地方
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            // 添加事件
            rc.Get<GameObject>("StartMatch").GetComponent<Button>().onClick.Add(OnStartMatch);

// 可以试图拿到玩家的相关数据，显示在主菜单某角落            
            // 获取玩家数据: 是通过Gate中转的, Gate 这里是小区的网关服
            long userId = ClientComponent.Instance.LocalPlayer.UserID; // 当地玩家：是单面登录成功的时候设置的
            C2G_GetUserInfo_Req c2G_GetUserInfo_Req = new C2G_GetUserInfo_Req() { UserID = userId };
            G2C_GetUserInfo_Ack g2C_GetUserInfo_Ack = await SessionComponent.Instance.Session.Call(c2G_GetUserInfo_Req) as G2C_GetUserInfo_Ack;
            // 显示用户信息
            rc.Get<GameObject>("NickName").GetComponent<Text>().text = g2C_GetUserInfo_Ack.NickName;
            rc.Get<GameObject>("Money").GetComponent<Text>().text = g2C_GetUserInfo_Ack.Money.ToString();
        }
    }
}
