using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace ETModel {

 // FileServer: 基本都是自己资源包文件服务器的客户端的逻辑，是不同的实现与封装，更为系统化模块化    
    [ObjectSystem]
    public class UiBundleDownloaderComponentAwakeSystem : AwakeSystem<BundleDownloaderComponent> {
        public override void Awake(BundleDownloaderComponent self) {
            self.bundles = new Queue<string>();
            self.downloadedBundles = new HashSet<string>();
            self.downloadingBundle = "";
        }
    }

    // 用来对比web端的资源，比较md5，对比下载资源
    public class BundleDownloaderComponent : Component {
        private VersionConfig remoteVersionConfig;
        
        public Queue<string> bundles;
        public long TotalSize;
        public HashSet<string> downloadedBundles;
        public string downloadingBundle;
        public UnityWebRequestAsync webRequest;

        public async Task StartAsync() {
            // 获取远程的Version.txt
            string versionUrl = "";
            try {
                using (UnityWebRequestAsync webRequestAsync = ComponentFactory.Create<UnityWebRequestAsync>()) {
// 把服务器端的资源包版本号： 定位到这个服务器端文件的位置　// F:\ettetris\Release\PC\StreamingAssets
                    versionUrl = GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + "Version.txt"; 
                    // Log.Debug(versionUrl);
                    await webRequestAsync.DownloadAsync(versionUrl); // 把服务器版本号文件下载下来到客户端了
 // 拿到异步调用（服务器到客户端）的返回结果，反序列化为VersionConfig对象，供客户端比对  MD5  ？                    
                    remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(webRequestAsync.Request.downloadHandler.text);
                    // Log.Debug(JsonHelper.ToJson(this.VersionConfig));
                }
            } 
            catch (Exception e) {
                throw new Exception($"url: {versionUrl}", e);
            }
            // 获取streaming目录的Version.txt
            VersionConfig streamingVersionConfig;
// 这里不该是：客户端本地的资源包版本号文件吗，也是通过如下方式读到的？是的　F:\ettetris\Release\ET_Data\StreamingAssets
            string versionPath = Path.Combine(PathHelper.AppResPath4Web, "Version.txt"); 
            using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>()) {
                await request.DownloadAsync(versionPath);
                streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
            }
            
            // 删掉远程不存在的文件
            DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.AppHotfixResPath);
            if (directoryInfo.Exists) {
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos) {
                    if (remoteVersionConfig.FileInfoDict.ContainsKey(fileInfo.Name)) {
                        continue;
                    }
                    if (fileInfo.Name == "Version.txt") {
                        continue;
                    }
                    
                    fileInfo.Delete();
                }
            } else {
                directoryInfo.Create();
            }
            // 应用本地的，也存在于服务器的文件：对比双端MD5，服务器文件信息存有其MD5码
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values) {
                // 对比md5
                string localFileMD5 = BundleHelper.GetBundleMD5(streamingVersionConfig, fileVersionInfo.File);
                if (fileVersionInfo.MD5 == localFileMD5) {
                    continue;
                }
 // 本地落后：与服务器的版本号不同的，就缓存标识到队列里，呆会儿就可以从服务器下载同步最近的到本地了                
                this.bundles.Enqueue(fileVersionInfo.File); // 这里没有看懂： 这个方法说是开始异步，但它只是这里更新了需要待下载的资源包链表，并不曾真正开始异步下载
                this.TotalSize += fileVersionInfo.Size;
            }
        }

        public int Progress {
            get {
                if (this.TotalSize == 0) {
                    return 0;
                }
                long alreadyDownloadBytes = 0;
                foreach (string downloadedBundle in this.downloadedBundles) {
                    long size = this.remoteVersionConfig.FileInfoDict[downloadedBundle].Size;
                    alreadyDownloadBytes += size;
                }
                if (this.webRequest != null) {
                    alreadyDownloadBytes += (long)this.webRequest.Request.downloadedBytes;
                }
                return (int)(alreadyDownloadBytes * 100f / this.TotalSize);
            }
        }
 // 一个一个的资源包下载
        public async Task DownloadAsync() {
            if (this.bundles.Count == 0 && this.downloadingBundle == "") {
                return;
            }
            try {
                while (true) {
                    if (this.bundles.Count == 0) {
                        break;
                    }
                    this.downloadingBundle = this.bundles.Dequeue(); // 遍历队列：每个落后于服务器，不同步的资源包都下载同步到本地
                    while (true) {
                        try {
                            using (this.webRequest = ComponentFactory.Create<UnityWebRequestAsync>()) {
                                await this.webRequest.DownloadAsync(GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + this.downloadingBundle);
                                byte[] data = this.webRequest.Request.downloadHandler.data;
                                string path = Path.Combine(PathHelper.AppHotfixResPath, this.downloadingBundle);
                                using (FileStream fs = new FileStream(path, FileMode.Create)) { // 这里是说，下载到本地后，是可以自动覆盖原版本落后的资源包的？
                                    fs.Write(data, 0, data.Length);
                                }
                            }
                        }
                        catch (Exception e) {
                            Log.Error($"download bundle error: {this.downloadingBundle}\n{e}");
                            continue;
                        }
                        break;
                    }
                    this.downloadedBundles.Add(this.downloadingBundle);
                    this.downloadingBundle = "";
                    this.webRequest = null;
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
