using System.Net;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {

    [BsonIgnoreExtraElements]
    public class InnerConfig: AConfigComponent {
        // 这也就是个最基础的配置呀
        [BsonIgnore]
        public IPEndPoint IPEndPoint { get; private set; } // 有个IP 网址
        
        public string Address { get; set; } // 地址？

        public override void EndInit() { // 初始化结束的时候，赋值
            this.IPEndPoint = NetworkHelper.ToIPEndPoint(this.Address);
        }
    }
}