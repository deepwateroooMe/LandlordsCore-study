using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ETModel {
    [ObjectSystem]
    public class DbSaveBatchTaskSystem : AwakeSystem<DBSaveBatchTask, List<ComponentWithId>, string, TaskCompletionSource<bool>> {
        public override void Awake(DBSaveBatchTask self, List<ComponentWithId> components, string collectionName, TaskCompletionSource<bool> tcs) {
            self.Components = components;
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }
    public sealed class DBSaveBatchTask : DBTask {
        public string CollectionName { get; set; }
        public List<ComponentWithId> Components; // 这里是：一批 IDs, 不止一个，所以称为批量批量操作
        public TaskCompletionSource<bool> Tcs;
    
        public override async Task Run() {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            foreach (ComponentWithId component in this.Components) { // 就是遍历：对每一个 Id 文档里的每一条进行执行
                if (component == null) {
                    continue;
                }
                try {
                    // 执行保存数据库任务: 这里它像是说，置换一个当前表中同ID的一条；如果表格中不存在，就插入这条信息
                    await dbComponent.GetCollection(this.CollectionName).ReplaceOneAsync(s => s.Id == component.Id, component, new UpdateOptions { IsUpsert = true });
                }
                catch (Exception e) {
                    Log.Debug($"{component.GetType().Name} {component.ToJson()} {e}");
                    this.Tcs.SetException(new Exception($"保存数据失败! {CollectionName} {this.Components.ListToString()}", e));
                }
            }
            this.Tcs.SetResult(true);
        }
    }
}