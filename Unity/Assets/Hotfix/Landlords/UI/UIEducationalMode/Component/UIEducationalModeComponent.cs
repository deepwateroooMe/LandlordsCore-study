using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix {

    [ObjectSystem]
    public class UIEducationalModeComponentAwakeSystem : AwakeSystem<UIEducationalModeComponent> {
        public override void Awake(UIEducationalModeComponent self) {
            self.Awake();
        }
    }

// 启蒙模式选择3 4 5 界面：
    public class UIEducationalModeComponent : Component {
        public void Awake() {
            Init();
        }
        // 初始化: 视图里控件的点击回调事件的注册，逻辑编写等
        // private async void Init() {
        private void Init() {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>(); // 这个UI类型，也必须得添加到这个系统被认得识别
 // 那个现在，就固定成为三种模式3 4 5，不作可能会扩展为1 2 6 7 的接口设计 
            // 添加事件：用一个 ToggleGroup脚本来控制单一选择.也可以不用它，自己控制逻辑（但是实现起来会相对难一点儿）
            int currToggle = 3;
            Toggle thre = rc.Get<GameObject>("Toggle3").GetComponent<Toggle>();
            Toggle four = rc.Get<GameObject>("Toggle4").GetComponent<Toggle>();
            Toggle five = rc.Get<GameObject>("Toggle5").GetComponent<Toggle>();
            thre.onValueChanged.AddListener((bool isOn)=> { OnToggleClick(toggle,isOn); });
 // 然后下面的这么写，又是在混逻辑，不是要什么与什么独立开来吗？            
            rc.Get<GameObject>("Toggle3").GetComponent<Toggle>().onValueChanged.AddListener((isSelected) => {
                if (!isSelected) {
                    return;
                }
                // var activeToggle = Active();
                // DoOnChange(activeToggle);
                // 这里需要触发下一步的回调：处理排它性，失活其它。。。
            });
 
    }
	
	private void OnToggleClick(Toggle toggle,bool isOn)
    {
        //TODO
    }


// 可以试图拿到玩家的相关数据，显示在主菜单某角落            
            // // 获取玩家数据: 按说应该是注册登录服的逻辑，或者是数据库服存放着用户信息，都是通过Gate中转的吗？
            // long userId = ClientComponent.Instance.LocalPlayer.UserID; // 当地玩家：是单面登录成功的时候设置的
            // C2G_GetUserInfo_Req c2G_GetUserInfo_Req = new C2G_GetUserInfo_Req() { UserID = userId };
            // G2C_GetUserInfo_Ack g2C_GetUserInfo_Ack = await SessionComponent.Instance.Session.Call(c2G_GetUserInfo_Req) as G2C_GetUserInfo_Ack;
            // // 显示用户信息
            // rc.Get<GameObject>("NickName").GetComponent<Text>().text = g2C_GetUserInfo_Ack.NickName;
            // rc.Get<GameObject>("Money").GetComponent<Text>().text = g2C_GetUserInfo_Ack.Money.ToString();
        }
        // public Toggle Active() {
        //     return ActiveToggles().FirstOrDefault();
        // }
        // protected virtual void DoOnChange(Toggle newactive) {
        //     var handler = OnChange;
        //     if (handler != null) handler(newactive);
        // }
        
        // 开始StartEducationalMode按钮事件
        // public async void OnStartMatch() {  // 点击事件回调的方法名，也是需要改的：EducationalMode async: 如果不是网络调用，还需要异步吗？
        public void OnStartMatch() {  // 点击事件回调的方法名，也是需要改的：EducationalMode async: 如果不是网络调用，还需要异步吗？
            // public void OnStartMatch() {  // 点击事件回调的方法名，也是需要改的：EducationalMode async: 如果不是网络调用，还需要异步吗？
            //  // 要在这里面写视图的切换吗？再去找逻辑
            // // UI room = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsRoom); // 装载新的UI视图，不需要进到这里面来
            // UI uiEducationalMode = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIEducationalMode); // 进入到三种选择里去
            // Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIEducationalMode);          // 卸载旧的UI视图
            // // 将房间设为匹配状态
            // room.GetComponent<LandlordsRoomComponent>().Matching = true;

            // try {
            //     // 发送开始匹配消息
            //     C2G_StartMatch_Req c2G_StartMatch_Req = new C2G_StartMatch_Req();
            //     G2C_StartMatch_Ack g2C_StartMatch_Ack = await SessionComponent.Instance.Session.Call(c2G_StartMatch_Req) as G2C_StartMatch_Ack;
            //     if (g2C_StartMatch_Ack.Error == ErrorCode.ERR_UserMoneyLessError) {
            //         Log.Error("余额不足"); // 就是说，当且仅当余额不足的时候才会出这个错误？
            //         return;
            //     }
            //     // 匹配成功了：切换到房间界面
            //     UI room = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsRoom); // 装载新的UI视图
            //     Game.Scene.GetComponent<UIComponent>().Remove(UIType.LandlordsLobby);          // 卸载旧的UI视图
            //     // Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIEducationalMode);          // 卸载旧的UI视图
            //     // 将房间设为匹配状态
            //     room.GetComponent<LandlordsRoomComponent>().Matching = true;
            // }
            // catch (Exception e) {
            //     Log.Error(e.ToStr());
            // }
        }
    }
}
