using System.Collections.Generic;
namespace ETModel {

    // 一个注册登录服下；可以有狠多个网关服，要记住他们的地址. 这里意思是说：如果想要配置和实现更优化的负载均衡网关服务器，可以对这个网关进行管理，以便进一步地优化。【目前自己并不考虑这些】
    public class RealmGateAddressComponent : Component {

        public readonly List<StartConfig> GateAddress = new List<StartConfig>();

        public StartConfig GetAddress() {
            // 这里的逻辑简单粗暴：随机分配。实际应用中，可以配置逻辑，谁等得比较久呀，哪个网关下最缺人等
            int n = RandomHelper.RandomNumber(0, this.GateAddress.Count); // 随机分配一个网关服
            return this.GateAddress[n];
        }
    }
}
