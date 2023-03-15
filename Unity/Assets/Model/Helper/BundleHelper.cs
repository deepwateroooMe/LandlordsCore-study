using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel {
    public static class BundleHelper {

        public static async Task DownloadBundle() {
            if (Define.IsAsync) {
                try { // 这里有个文件服务器的地址，比对双端资源包的MD5码，下载必要的资源包  
                    using (BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.AddComponent<BundleDownloaderComponent>()) {
                        await bundleDownloaderComponent.StartAsync();   // 开始双端的码表比对，并标记需要下载的
                        Game.EventSystem.Run(EventIdType.LoadingBegin); // 一个特定的事件：加载开始
                        await bundleDownloaderComponent.DownloadAsync();
                    }
                    Game.EventSystem.Run(EventIdType.LoadingFinish); // 一个特定的事件：加载结束
                    
                    Game.Scene.GetComponent<ResourcesComponent>().LoadOneBundle("StreamingAssets");
 // 不知道下面的，说的是什么意思
                    ResourcesComponent.AssetBundleManifestObject = (AssetBundleManifest)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("StreamingAssets", "AssetBundleManifest");
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }
        // 客户端有调用：这里若是客户端调用，计算的方式奇怪【当客户端本地的资源包文件不存在的时候？】
        public static string GetBundleMD5(VersionConfig streamingVersionConfig, string bundleName) {
            string path = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            if (File.Exists(path)) { // 如果文件存在，则重新计算MD5 码【以防文件更新过，但文件更新过，客户端本地的MD5 码表文件不曾同步更新？】
                return MD5Helper.FileMD5(path);
            }
            // 客户端本地不存在文件：从客户端的码表文件中去读取最后最新纪录？【客户端本地资源包文件都不存在了，只读一个MD5 码，若与服务器端相同，会不再下载，拿这个MD5 码有什么用呢？】
            if (streamingVersionConfig.FileInfoDict.ContainsKey(bundleName)) {
                return streamingVersionConfig.FileInfoDict[bundleName].MD5;    
            }
            return "";
        }
    }
}
