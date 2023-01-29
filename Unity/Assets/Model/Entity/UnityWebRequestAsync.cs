using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace ETModel {

    [ObjectSystem]
    public class UnityWebRequestUpdateSystem : UpdateSystem<UnityWebRequestAsync> {
        public override void Update(UnityWebRequestAsync self) {
            self.Update();
        }
    }
 // 把游戏端的异步网络请求：包装成为异步任务    
    public class UnityWebRequestAsync : Component {
        public UnityWebRequest Request;
        public bool isCancel;
        public TaskCompletionSource<bool> tcs;
        
        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.Request?.Dispose();
            this.Request = null;
            this.isCancel = false;
        }
        public float Progress {
            get {
                if (this.Request == null) {
                    return 0;
                }
                return this.Request.downloadProgress;
            }
        }
        public ulong ByteDownloaded {
            get {
                if (this.Request == null) {
                    return 0;
                }
                return this.Request.downloadedBytes;
            }
        }
        public void Update() {
            if (this.isCancel) {
                this.tcs.SetResult(false);
                return;
            }
            
            if (!this.Request.isDone) {
                return;
            }
            if (!string.IsNullOrEmpty(this.Request.error)) {
                this.tcs.SetException(new Exception($"request error: {this.Request.error}"));
                return;
            }
            this.tcs.SetResult(true);
        }
        public Task<bool> DownloadAsync(string url) {
            this.tcs = new TaskCompletionSource<bool>();
            
            url = url.Replace(" ", "%20"); // 小细节： 大致是一个空格替换成某个符号
            this.Request = UnityWebRequest.Get(url);
            this.Request.SendWebRequest();
            
            return this.tcs.Task; // 这里返回的就是网络请求异步任务的结果
        }
    }
}
