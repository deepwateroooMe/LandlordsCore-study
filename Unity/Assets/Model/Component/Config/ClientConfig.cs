using System.Net;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {

    // 客户端配置： 最主要就是IPAdress ?
    public class ClientConfig: AConfigComponent {

        public string Address { get; set; }
    }
}