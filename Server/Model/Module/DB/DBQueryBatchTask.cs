using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ETModel {
    [ObjectSystem]
    public class DbQueryBatchTaskSystem : AwakeSystem<DBQueryBatchTask, List<long>, string, TaskCompletionSource<List<ComponentWithId>>> {
        public override void Awake(DBQueryBatchTask self, List<long> idList, string collectionName, TaskCompletionSource<List<ComponentWithId>> tcs) {
            self.IdList = idList; // Ids: 是作为参数传入这个组件的
            self.CollectionName = collectionName; //  数据库表的名字
            self.Tcs = tcs;
        }
    }
    public sealed class DBQueryBatchTask : DBTask {
        public string CollectionName { get; set; }
        public List<long> IdList { get; set; } // 传进来的参数：是条款标识的集合，链表
        public TaskCompletionSource<List<ComponentWithId>> Tcs { get; set; }
        public override async Task Run() {
            DBCacheComponent dbCacheComponent = Game.Scene.GetComponent<DBCacheComponent>();
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            List<ComponentWithId> result = new List<ComponentWithId>();
            try {
                // 执行查询数据库任务
                foreach (long id in IdList) { // 遍历 IDs: 一个又一个
                    ComponentWithId component = dbCacheComponent.GetFromCache(this.CollectionName, id); // 先，试图从缓存中直接读取
                    if (component == null) {
                        IAsyncCursor<ComponentWithId> cursor = await dbComponent.GetCollection(this.CollectionName).FindAsync((s) => s.Id == id); // 异步查找
                        component = await cursor.FirstOrDefaultAsync(); // 从上面的返回类型里再拿第一个或是缺省值，也就成了为每个ID返回表中的靠前的一个条目
                    }
                    if (component == null) {
                        continue;
                    }
                    result.Add(component);
                }
                this.Tcs.SetResult(result);
            }
            catch (Exception e) {
                this.Tcs.SetException(new Exception($"查询数据库异常! {this.CollectionName} {IdList.ListToString()}", e));
            }
        }
    }
}