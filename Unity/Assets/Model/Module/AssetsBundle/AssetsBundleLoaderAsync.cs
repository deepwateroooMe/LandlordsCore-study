using System.IO;
using System.Threading.Tasks;
using UnityEngine;
namespace ETModel {

    [ObjectSystem]
    public class AssetsBundleLoaderAsyncSystem : UpdateSystem<AssetsBundleLoaderAsync> {
        public override void Update(AssetsBundleLoaderAsync self) {
            self.Update();
        }
    }

    public class AssetsBundleLoaderAsync : Component {

        private AssetBundleCreateRequest request;
        private TaskCompletionSource<AssetBundle> tcs;

        public void Update() {
            if (!this.request.isDone) {
                return;
            }
            TaskCompletionSource<AssetBundle> t = tcs;
            t.SetResult(this.request.assetBundle);
        }
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
        }
        public Task<AssetBundle> LoadAsync(string path) {
            this.tcs = new TaskCompletionSource<AssetBundle>(); // 对象任务池里去抓一个返回AssetBundle类型的任务实例
            this.request = AssetBundle.LoadFromFileAsync(path); // 请求，传入链接参数
            return this.tcs.Task; // 返回异步任务的执行结果
        }
    }
}
