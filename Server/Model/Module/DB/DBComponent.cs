using MongoDB.Driver;
namespace ETModel {

 // 这个，应该主要是在服务器端，有个专门的数据库服务器吗？ 是的   
    [ObjectSystem]
    public class DbComponentSystem : AwakeSystem<DBComponent> {
        public override void Awake(DBComponent self) {
            self.Awake();
        }
    }

// 连接mongodb
    public class DBComponent : Component {

        public MongoClient mongoClient; // 它是以客户端的形式去连接MongoDB数据库中心服务器，所以这里，它仍然定义的是(MongoDB数据库中心服)的客户端连接操作逻辑？
        public IMongoDatabase database;

        public void Awake() {
            DBConfig config = Game.Scene.GetComponent<StartConfigComponent>().StartConfig.GetComponent<DBConfig>();
            string connectionString = config.ConnectionString;
            mongoClient = new MongoClient(connectionString);
            this.database = this.mongoClient.GetDatabase(config.DBName); // 这里拿到的就是，项目配置文件中配置过的MongoDB的数据库MeMeMe
        }

        public IMongoCollection<ComponentWithId> GetCollection(string name) {
            return this.database.GetCollection<ComponentWithId>(name);
        }
    }
}