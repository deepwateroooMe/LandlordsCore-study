using System;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace ETModel {

    [ObjectSystem]
    public class DBQueryTaskSystem : AwakeSystem<DBQueryTask, string, TaskCompletionSource<ComponentWithId>> {

        public override void Awake(DBQueryTask self, string collectionName, TaskCompletionSource<ComponentWithId> tcs) {
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryTask : DBTask {

        public string CollectionName { get; set; }
        public TaskCompletionSource<ComponentWithId> Tcs { get; set; }

        public override async Task Run() {
            DBCacheComponent dbCacheComponent = Game.Scene.GetComponent<DBCacheComponent>();
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            // 执行查询前先看看cache中是否已经存在
            ComponentWithId component = dbCacheComponent.GetFromCache(this.CollectionName, this.Id);
            if (component != null) {
                this.Tcs.SetResult(component);
                return;
            }
            try {
                // 执行查询数据库任务:首先调用API-FindAsync获取一个数据游标。然后调用第二个API-FirstOrDefaultAsync获取真正的数据类
                IAsyncCursor<ComponentWithId> cursor = await dbComponent.GetCollection(this.CollectionName).FindAsync((s) => s.Id == this.Id);
                component = await cursor.FirstOrDefaultAsync(); // 返回的是第一条，或是不存在任何数据情况下的缺少值
                this.Tcs.SetResult(component);
            }
            catch (Exception e) {
                this.Tcs.SetException(new Exception($"查询数据库异常! {CollectionName} {Id}", e));
            }
        }
    }
}