using System.Collections.Generic;
namespace ETModel {

    public class RealmGateAddressComponent : Component {
        // 一个注册登录服下；可以有狠多个网关服，要记住他们的地址
        public readonly List<StartConfig> GateAddress = new List<StartConfig>();

        public StartConfig GetAddress() {
            // 这里的逻辑简单粗暴：随机分配。实际应用中，可以配置逻辑，谁等得比较久呀，哪个网关下最缺人等
            int n = RandomHelper.RandomNumber(0, this.GateAddress.Count);
            return this.GateAddress[n];
        }
    }
}
