using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
namespace ETHotfix {

    [ObjectSystem]
    public class LandlordsLoginComponentAwakeSystem : AwakeSystem<LandlordsLoginComponent> {
        public override void Awake(LandlordsLoginComponent self) {
            self.Awake();
        }
    }
    // 注册、登录界面组件：
    public class LandlordsLoginComponent : Component {
        // 账号输入框
        private InputField account;
        // 密码输入框
        private InputField password;
        // 提示文本
        private Text prompt;
        // 是否正在登录中（避免登录请求还没响应时连续点击登录）
        private bool isLogining;
        // 是否正在注册中（避免登录请求还没响应时连续点击注册）
        private bool isRegistering;

        public void Awake() {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            // 热更测试
            Text hotfixPrompt = rc.Get<GameObject>("HotfixPrompt").GetComponent<Text>();
#if ILRuntime
            hotfixPrompt.text = "斗地主4.0.0 (ILRuntime模式)";
#else
            hotfixPrompt.text = "斗地主4.0.0 (Mono模式)";
#endif
            // 绑定关联对象
            account = rc.Get<GameObject>("Account").GetComponent<InputField>();
            password = rc.Get<GameObject>("Password").GetComponent<InputField>();
            prompt = rc.Get<GameObject>("Prompt").GetComponent<Text>();
            // 添加事件
            rc.Get<GameObject>("LoginButton").GetComponent<Button>().onClick.Add(OnLogin);
            rc.Get<GameObject>("RegisterButton").GetComponent<Button>().onClick.Add(OnRegister);
        }
        // 设置提示
        // <param name="str"></param>
        public void SetPrompt(string str) {
            this.prompt.text = str;
        }
        // 登录按钮事件
        public async void OnLogin() {
            if (isLogining || this.IsDisposed) {
                return;
            }
 // 游戏中哪里什么地方加载了这个对外网络组件？ ETModel 加载的时候就会自动加载这个组件了
            NetOuterComponent netOuterComponent = Game.Scene.ModelScene.GetComponent<NetOuterComponent>();
            // 设置登录中状态
            isLogining = true;
            Session sessionWrap = null;
            try {
                // 创建登录服务器连接
                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
                ETModel.Session session = netOuterComponent.Create(connetEndPoint);
                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);
                sessionWrap.session.GetComponent<SessionCallbackComponent>().DisposeCallback += s =>  { // 先注册：回收的回调
                    if (Game.Scene.GetComponent<UIComponent>()?.Get(UIType.LandlordsLogin) != null) {
                        prompt.text = "断开连接";
                        isLogining = false;
                    }
                };
                // 发送登录请求
                prompt.text = "正在登录中....";
                // 客户端发送帐号密码给Realm注册登录服，login server验证，并且等待login响应消息返回，login会分配一个网关给客户端
                R2C_Login_Ack r2C_Login_Ack = await sessionWrap.Call(new C2R_Login_Req() { Account = account.text, Password = password.text }) as R2C_Login_Ack;
                prompt.text = "";
                if (this.IsDisposed) {
                    return;
                }
                if (r2C_Login_Ack.Error == ErrorCode.ERR_LoginError) {
                    prompt.text = "登录失败,账号或密码错误";
                    password.text = "";
                    // 断开验证服务器的连接
                    sessionWrap.Dispose();
                    // 设置登录处理完成状态
                    isLogining = false;
                    return;
                }
                // 加个讲解链接：https://github.com/Viagi/LandlordsCore/blob/master/Doc/%E7%BD%91%E7%BB%9C%E5%B1%82%E8%AE%BE%E8%AE%A1.md
                // 可以看到：登录完loginserver，立即登录gateserver，登录完成后又查询了玩家的物品信息，整个流程看起来非常连贯，
                // 假如没有async await，这段代码就得拆成至少4块放到4个函数中。分布式服务器连续rpc调用非常多，没有async await这种协程的语法支持是不可想像的。

                // 创建Gate服务器连接：客户端连接网关 ？？？
                connetEndPoint = NetworkHelper.ToIPEndPoint(r2C_Login_Ack.Address); // 可是这里不明明写的是Realm 2 Client的吗？怎么是去Gate网关的呢？
                ETModel.Session gateSession = netOuterComponent.Create(connetEndPoint);
                Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession); // 重新工厂实例化一个去网关服的会话消息
                // SessionWrap添加连接断开组件，用于处理客户端连接断开
                Game.Scene.GetComponent<SessionComponent>().Session.AddComponent<SessionOfflineComponent>(); // 这个例子可以明白：框架的定义在哪里，游戏中如何
                Game.Scene.ModelScene.AddComponent<ETModel.SessionComponent>().Session = gateSession; // 这个会话存放的地方，客户端维护的唯一会话框；与且仅与网关服会话
                // 登录Gate服务器：这里跟我查找的理论图稍有不同，它说先从注册登录服返回，可以拿到一个Key,再用这个Key去连网关gate服
                G2C_LoginGate_Ack g2C_LoginGate_Ack = await SessionComponent.Instance.Session.Call(new C2G_LoginGate_Req() { Key = r2C_Login_Ack.Key }) as G2C_LoginGate_Ack;
                if (g2C_LoginGate_Ack.Error == ErrorCode.ERR_ConnectGateKeyError) {
                    prompt.text = "连接网关服务器超时";
                    password.text = "";
                    Game.Scene.GetComponent<SessionComponent>().Session.Dispose();
                    // 断开验证服务器的连接
                    sessionWrap.Dispose();
                    // 设置登录处理完成状态
                    isLogining = false;
                    return;
                }
                Log.Info("登录成功");
                // 保存本地玩家
                User user = ETModel.ComponentFactory.CreateWithId<User, long>(g2C_LoginGate_Ack.PlayerID, g2C_LoginGate_Ack.UserID); // 拿到PlayerID UserID
// 斗地主客户端的第一个自定义组件ClientComponent 定义了静态唯一实例Instance和User成员LocalPlayer, 在登陆成功后设置改实例为当前玩家                
                ClientComponent.Instance.LocalPlayer = user;
                // 跳转到大厅界面
                Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsLobby);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.LandlordsLogin);
            }
            catch (Exception e) {
                prompt.text = "登录异常";
                Log.Error(e.ToStr());
                // 断开验证服务器的连接
                sessionWrap.Dispose();
                // 设置登录处理完成状态
                isLogining = false;
            }
        }
        public async void OnRegister() { // 注册按钮事件
            if (isRegistering || this.IsDisposed) {
                return;
            }
            // 设置登录中状态
            isRegistering = true;
            Session sessionWrap = null;
            prompt.text = "";
            try {
                // 创建登录服务器连接
                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
                ETModel.Session session = Game.Scene.ModelScene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);
                // 发送注册请求
                prompt.text = "正在注册中....";
                 // 调用的时候是， Client 2 Realm注册登录服，返回的是 Realm 2 Client  IResponse 转化为R2C_Register_Ack协议
                R2C_Register_Ack r2C_Register_Ack = await sessionWrap.Call(new C2R_Register_Req() { Account = account.text, Password = password.text }) as R2C_Register_Ack;
                prompt.text = "";
                if (this.IsDisposed) {
                    return;
                }
                if (r2C_Register_Ack.Error == ErrorCode.ERR_AccountAlreadyRegister) { // 只处理了这一个相关的错误，其它没管
                    prompt.text = "注册失败，账号已被注册"; // 所以，是说，这个版本里没有顶号的逻辑？可以去ET7 里找一找这个部分相关的逻辑
                    account.text = "";
                    password.text = "";
                    return;
                }
                // 注册成功自动登录
                OnLogin();
            }
            catch (Exception e) {
                prompt.text = "注册异常";
                Log.Error(e.ToStr());
            }
            finally {
                // 断开验证服务器的连接: 这里说这个客户端，以后就是与网关服Gate往返消息，与注册登录服Realm这里连接就断开了。网关服大概主要就转消息
                sessionWrap?.Dispose();
                // 设置注册处理完成状态
                isRegistering = false;
            }
        }
    }
}
