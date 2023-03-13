namespace ETModel {

    public class GlobalProto {

        public string AssetBundleServerUrl; // 热更新资源包文件服务器链接地址
        public string Address;

        public string GetUrl() { // 这里说，根据客户端平台的不同，从相应的服务器文件夹中来读取热更新资源包资源
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
            Log.Debug(url); // 这个消息不好，不知道打印的是什么
            
// 这个客户端，存在严重的读写延迟，需要优化。【亲爱的表哥，活宝妹一定要嫁的亲爱的表哥！！！】          
            return url;
        }
    }
}
