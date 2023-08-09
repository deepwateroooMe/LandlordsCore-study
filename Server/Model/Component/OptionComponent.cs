using System;
using CommandLine;
namespace ETModel {
    [ObjectSystem]
    public class OptionComponentSystem : AwakeSystem<OptionComponent, string[]> {
        public override void Awake(OptionComponent self, string[] a) {
            self.Awake(a);
        }
    }
    // 那么，现在再看这里，看见的是什么呢？一个框架开发者，程序员的成长！！
    // 亲爱的表哥眼中的弱弱活宝妹，在亲爱的表哥的一再鼓励下，活宝妹只会成长得更快呀！！爱表哥，爱生活！！！任何时候，亲爱的表哥的活宝妹就是一定要、一定会嫁给活宝妹的亲爱的表哥！！！爱表哥，爱生活！！！
    public class OptionComponent : Component {
        public Options Options { get; set; }
        public void Awake(string[] args) {
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(error => throw new Exception($"命令行格式错误!"))
                .WithParsed(options => { Options = options; });
        }
    }
}
