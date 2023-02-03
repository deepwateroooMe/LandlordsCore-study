using System;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace ETModel {

    [ObjectSystem]
    public class DbSaveTaskAwakeSystem : AwakeSystem<DBSaveTask, ComponentWithId, string, TaskCompletionSource<bool>> {

        public override void Awake(DBSaveTask self, ComponentWithId component, string collectionName, TaskCompletionSource<bool> tcs) {
            self.Component = component;
            self.CollectionName = collectionName;
            self.Tcs = tcs;
        }
    }

// 异步保存MongoDB数据库中某个表格一条内容的 异步任务包装: 是不是找个包装调用的地方看一下？
    public sealed class DBSaveTask : DBTask {

        public ComponentWithId Component;
        public string CollectionName { get; set; }
        public TaskCompletionSource<bool> Tcs;

        public override async Task Run() {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            try {
                // 执行保存数据库任务
                await dbComponent.GetCollection(this.CollectionName).ReplaceOneAsync(s => s.Id == this.Component.Id, this.Component, new UpdateOptions {IsUpsert = true});
                this.Tcs.SetResult(true);
            }
            catch (Exception e) {
                this.Tcs.SetException(new Exception($"保存数据失败!  {CollectionName} {Id}", e));
            }
        }
    }
}