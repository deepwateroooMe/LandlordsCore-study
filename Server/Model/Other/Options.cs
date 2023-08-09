#if SERVER
using CommandLine;
#endif
namespace ETModel {
    public class Options { // 【爱表哥，爱生活！！！任何时候，亲爱的表哥的活宝妹就是一定要、一定会嫁给活宝妹的亲爱的表哥！！！爱表哥，爱生活！！！】

        [Option("appId", Required = false, Default = 1)]
        public int AppId { get; set; }
        
        // 没啥用，主要是在查看进程信息能区分每个app.exe的类型, 前面的话是，自己写的吗？不敢相信
        [Option("appType", Required = false, Default = AppType.Manager)]
        public AppType AppType { get; set; }
        // 这里是，自动从配置文件加载进来的 config 变量。比如下面的 .txt 文件，加载的是一个试运行的、全服的配置 
        [Option("config", Required = false, Default = "../Config/StartConfig/LocalAllServer.txt")]
        public string Config { get; set; }
    }
}