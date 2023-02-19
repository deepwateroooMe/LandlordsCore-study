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
    public sealed class DBSaveTask : DBTask { // 感觉，这里就是，根据最普通数据库操作 CRUD 来区分定义的不同数据库操作任务系统 ?

        public ComponentWithId Component;
        public string CollectionName { get; set; }
        public TaskCompletionSource<bool> Tcs;

        public override async Task Run() {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            try {
                // 执行保存数据库任务：设置的UpdateOptions IsUpsert=true 说明了，如果当前表格不存在这个 id, 则直接将这条待操作数据添加进该表格（文档）
                await dbComponent.GetCollection(this.CollectionName).ReplaceOneAsync(s => s.Id == this.Component.Id, this.Component, new UpdateOptions {IsUpsert = true});
                this.Tcs.SetResult(true);
            }
            catch (Exception e) {
                this.Tcs.SetException(new Exception($"保存数据失败!  {CollectionName} {Id}", e));
            }
        }
        // UpdateOptions
        // BypassDocumentValidation：Gets or sets a value indicating whether to bypass document validation.（是否绕过文档验证获取或设置一个值。）
        // IsUpsert：Gets or sets a value indicating whether to insert the document if it doesn’t already exist.（获取或设置一个值，该值指示在文档不存在该值时是否插入该文档（创建值）。）

    }
}