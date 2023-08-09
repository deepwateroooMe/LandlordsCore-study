using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {
    // 【爱表哥，爱生活！！！任何时候，亲爱的表哥的活宝妹就是一定要、一定会嫁给活宝妹的亲爱的表哥！！！爱表哥，爱生活！！！】

    public class StartConfig : Entity {

        public int AppId { get; set; } // 进程号

        [BsonRepresentation(BsonType.String)] // 大晚上的，不太看得懂这个标签是什么意思，暂时放过它。。。
        public AppType AppType { get; set; } // 进程类型

        public string ServerIP { get; set; } // 进程所在，物理机的IP 地址
    }
}
