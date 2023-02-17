namespace ETModel {

    // SessionComponent 组件用于存储Seesion，
    // 目前客户端只会存在一个在用的session。就是说，每个客户端有且仅拥有一个 session  
    // 登录后获得的GateSession会存储在这个组件中。会在之后的对服务器端的调用中使用这个session。
    // 也是因为登录之后的客户端，都是与网关服直接打交道中转，所以这一个会话框就够了。
    // 就如活宝妹只需要亲爱的亲爱的表哥，活宝妹一定要嫁的亲爱的表哥！！！一个会话框也就可以了
    [ObjectSystem]
    public class SessionComponentAwakeSystem : AwakeSystem<SessionComponent> {
        public override void Awake(SessionComponent self) {
            self.Awake();
        }
    }
    public class SessionComponent: Component {
        public static SessionComponent Instance; // 单例：每个客户端的单例
        public Session Session;

        public void Awake() {
            Instance = this;
        }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.Session.Dispose();
            this.Session = null;
            Instance = null;
        }
    }
}
