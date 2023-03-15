using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace ETModel {

    // 客户端资源包：下载服务器端MD5 码表文件; 与服务器端的码表比对，标记需要下载的；异步下载相关资源包
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
        private VersionConfig remoteVersionConfig; // 文件服务器器、客户端里所有资源包文件的统一单例管理 
        
        public Queue<string> bundles; // 所有【需要从服务器下载更新的】资源包
        public long TotalSize;
        public HashSet<string> downloadedBundles; // 下载完了的资源包
        public string downloadingBundle;          // 正在下载的当前资源包
        public UnityWebRequestAsync webRequest;

        public async Task StartAsync() {
            // 一、获取服务器端的资源包MD5 码表文件：远程的Version.txt
            string versionUrl = "";
            try {
                using (UnityWebRequestAsync webRequestAsync = ComponentFactory.Create<UnityWebRequestAsync>()) {
                    // 定位：码表文件： 具体到这个项目的目录地址是在：F:\LandlordsCore\Release\PC\StreamingAssets
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
            // 二、获取客户端本地的资源包码表文件：streaming目录的Version.txt
            VersionConfig streamingVersionConfig; // F:\LandlordsCore\Unity\Assets\StreamingAssets
            string versionPath = Path.Combine(PathHelper.AppResPath4Web, "Version.txt"); // 这里不该是：  客户端本地的资源包版本号文件吗，也是通过如下方式读到的？
            using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>()) { // 使用的是：它封装过了的ETTask 异步任何回调的方式
                await request.DownloadAsync(versionPath);
                streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
            }
            // 三、根据服务器的码表文件，同步客户端本地配置：删掉远程服务器中不存在的本地文件
            DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.AppHotfixResPath);
            if (directoryInfo.Exists) { // 本地存在资源包文件目录：
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos) {
                    if (remoteVersionConfig.FileInfoDict.ContainsKey(fileInfo.Name)) {
                        continue;
                    }
                    if (fileInfo.Name == "Version.txt") {
                        continue;
                    }
                    // 将远程服务器中不存在的本地文件：删除掉
                    fileInfo.Delete();
                }
            } else { // 本地不存在资源包文件目录
                directoryInfo.Create();
            }
            // 四、标记客户端本地文件中：MD5 码比对落后于服务器的资源包文件 
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values) { // 遍历服务器端资源包文件信息：
                // 对比md5
// 【特殊：两端 md5 相同，但什么原因，误操作？客户端本地文件丢失了】不再下载，接下来会发生什么？【虽然这种情况应该不至于存在】
                string localFileMD5 = BundleHelper.GetBundleMD5(streamingVersionConfig, fileVersionInfo.File); 
                if (fileVersionInfo.MD5 == localFileMD5) {
                    continue;
                }
                // 标记加入队列：本地落后：与服务器的版本号不同的，就缓存标识到队列里，呆会儿就可以从服务器下载同步最近的到本地了                
// 这个方法说是开始异步，但它本质上只是，开始异步比对双端的Md5 码表，只是这里更新了需要待下载的资源包队列，并不曾真正开始异步下载
                this.bundles.Enqueue(fileVersionInfo.File);  // 所有【需要从服务器下载更新的】资源包
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
        public async Task DownloadAsync() { // 前面的双端码表比对，标记好了客户端本地需要下载的资源包文件队列，这里真正执行异步下载任务
            if (this.bundles.Count == 0 && this.downloadingBundle == "") {
                return;
            }
            try {
                while (true) {
                    if (this.bundles.Count == 0) {
                        break;
                    }
                    this.downloadingBundle = this.bundles.Dequeue(); // 遍历队列：每个落后于服务器，不同步的资源包都下载同步到本地。当前正在下载的资源包
                    while (true) {
                        try {
                            using (this.webRequest = ComponentFactory.Create<UnityWebRequestAsync>()) { // 下面的调用：需要传入远程服务器对应资源包文件的目录地址：文件夹＋文件名
                                await this.webRequest.DownloadAsync(GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + this.downloadingBundle);
// 这里没看懂：使用了封装成了异步任务的网络调用，它的数据是如何这么拿到的？【这里是重点】异步网络调用，等待异步网络请求完成，并返回数据；拿读数据的方式奇特；把拿到的数据写进资源包文件
                                // 【就不存在资源包的压缩解压什么的了】：是资源包的压缩解压，还是RPC 进程间消息的压缩与解压？
                                byte[] data = this.webRequest.Request.downloadHandler.data; // Unity 系统内部封装，拿到的异步网络调用数据 
                                string path = Path.Combine(PathHelper.AppHotfixResPath, this.downloadingBundle); // 当前下载资源包，在客户端的保存存放目录文件名位置
                                using (FileStream fs = new FileStream(path, FileMode.Create)) { // 这里是说，下载到本地后，是可以自动覆盖原版本落后的资源包的？【原陈旧版本文件不曾删除过】
                                    fs.Write(data, 0, data.Length);
                                }
                            }
                        }
                        catch (Exception e) { // 捕获：某个异步网络请求下载资源包过程中，可能存在的异常
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
