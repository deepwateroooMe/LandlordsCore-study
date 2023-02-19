 using MongoDB.Bson.Serialization.Attributes;
namespace ETModel {
    // 数据库里面，它可能就只，存放了这两张表
    // 账号信息
    [BsonIgnoreExtraElements]
    public class AccountInfo : Entity {
        // 用户名
        public string Account { get; set; }
        // 密码
        public string Password { get; set; }
    }
}
