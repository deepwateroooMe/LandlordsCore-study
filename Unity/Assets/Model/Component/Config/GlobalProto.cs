namespace ETModel {

    public class GlobalProto {

        public string AssetBundleServerUrl; // 热更新资源包文件服务器链接地址
        public string Address;

        public string GetUrl() {
            string url = this.AssetBundleServerUrl;
#if UNITY_ANDROID
            url += "Android/";
#elif UNITY_IOS
            url += "IOS/";
#elif UNITY_WEBGL
            url += "WebGL/";
#elif UNITY_STANDALONE_OSX
            url += "MacOS/";
#else
            url += "PC/";
#endif
            Log.Debug(url);
            return url;
        }
    }
}
