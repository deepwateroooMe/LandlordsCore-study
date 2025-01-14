﻿using UnityEngine;
namespace ETModel {
    public static class PathHelper {     // <summary>

        // 应用程序外部资源路径存放路径(热更新资源路径)
        public static string AppHotfixResPath {
            get {
                string game = Application.productName;
                string path = AppResPath;
                if (Application.isMobilePlatform) {
                    path = $"{Application.persistentDataPath}/{game}/";
                }
                return path;
            }
        }

        // 应用程序内部资源路径存放路径
        public static string AppResPath {
            get {
                return Application.streamingAssetsPath;
            }
        }

// 应用程序内部资源路径存放路径(www/webrequest专用)
        public static string AppResPath4Web {
            get {
#if UNITY_IOS
                return $"file:// {Application.streamingAssetsPath}";
#else
                return Application.streamingAssetsPath;
#endif
            }
        }
    }
}
