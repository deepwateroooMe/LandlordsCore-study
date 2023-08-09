using System.Net;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {

    [BsonIgnoreExtraElements]
    public class OuterConfig: AConfigComponent {
        // 外网配置：是一个传输路径上，两个端点的地址？
        
        public string Address { get; set; }
        public string Address2 { get; set; }
    }
}