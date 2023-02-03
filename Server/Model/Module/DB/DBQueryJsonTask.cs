using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace ETModel {

    [ObjectSystem]
    public class DBQueryJsonTaskAwakeSystem : AwakeSystem<DBQueryJsonTask, string, string, TaskCompletionSource<List<ComponentWithId>>> {
        public override void Awake(DBQueryJsonTask self, string collectionName, string json, TaskCompletionSource<List<ComponentWithId>> tcs) {
            self.CollectionName = collectionName;
            self.Json = json;
            self.Tcs = tcs;
        }
    }

    public sealed class DBQueryJsonTask : DBTask {

        public string CollectionName { get; set; }
        public string Json { get; set; }
        public TaskCompletionSource<List<ComponentWithId>> Tcs { get; set; }
        
        public override async Task Run() {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            try {
                // 执行查询数据库任务: 这里是通过Json字符串的某种逻辑(把它怎么弄成下表格里数据的过滤筛选标准)实现从表格里查询的
                FilterDefinition<ComponentWithId> filterDefinition = new JsonFilterDefinition<ComponentWithId>(this.Json);
                IAsyncCursor<ComponentWithId> cursor = await dbComponent.GetCollection(this.CollectionName).FindAsync(filterDefinition);
                List<ComponentWithId> components = await cursor.ToListAsync(); // 因为查询的结果可能不止一条，所以返回的是条款集合的链表
                this.Tcs.SetResult(components);
            }
            catch (Exception e) {
                this.Tcs.SetException(new Exception($"查询数据库异常! {CollectionName} {this.Json}", e));
            }
        }
    }
}