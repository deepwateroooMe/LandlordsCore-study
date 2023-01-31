namespace ETModel {

    // 客户端使用的session都是通过这个组件创建的。
    // 需要切换网络协议或者数据序列化方式就在这个组件的Awake方法中指定对应的参数就可以了。
    public class NetOuterComponent : NetworkComponent {

        public NetworkProtocol Protocol = NetworkProtocol.TCP;
    }
}