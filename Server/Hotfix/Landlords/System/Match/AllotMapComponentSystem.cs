gusing ETModel;
namespace ETHotfix {
    [ObjectSystem]
    public class AllotMapComponentStartSystem : StartSystem<AllotMapComponent> {
        public override void Start(AllotMapComponent self) {
            self.Start();
        }
    }
    public static class AllotMapComponentSystem {

        public static void Start(this AllotMapComponent self) {
            StartConfig[] startConfigs = self.GetParent<Entity>().GetComponent<StartConfigComponent>().GetAll();
            foreach (StartConfig config in startConfigs) {
                if (!config.AppType.Is(AppType.Map)) {
                    continue;
                }
                self.MapAddress.Add(config);
            }
        }
        // 随机获取一个房间服务器地址: 前面理解到同一个登录注册服下可以有很多的网关服，这里是斗地主游戏里，可以有很多个房间服吗？理解概念困难
        public static StartConfig GetAddress(this AllotMapComponent self) {
            int n = RandomHelper.RandomNumber(0, self.MapAddress.Count);
            return self.MapAddress[n];
        }
    }
}
