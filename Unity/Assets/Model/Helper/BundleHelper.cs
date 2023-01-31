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
                        await bundleDownloaderComponent.StartAsync();
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

        public static string GetBundleMD5(VersionConfig streamingVersionConfig, string bundleName) {
            string path = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            if (File.Exists(path)) {
                return MD5Helper.FileMD5(path);
            }
            
            if (streamingVersionConfig.FileInfoDict.ContainsKey(bundleName)) {
                return streamingVersionConfig.FileInfoDict[bundleName].MD5;    
            }
            return "";
        }
    }
}
