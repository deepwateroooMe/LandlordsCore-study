using System;
using System.Collections.Generic;
namespace ETHotfix {

    public static partial class UIType {
        public const string Root = "Root";
        public const string UILogin = "UILogin"; // 注册 登录 界面
        public const string UILobby = "UILobby"; // 主菜单　三选项

// 上面的界面远远不够呀。。。
        public const string UIEducationalMode = "UIEducationalMode"; 

        public const string UIEducational = "UIEducational"; 
 // 怎么再把它细化为：三　四　五方格呢？ 应该是要用同一接口的不同实现，完全重复写三个系统会把人弄死的。。。。。
        public const string UIGridThree = "UIGridThree";
        public const string UIGridFour = "UIGridFour"; 
        public const string UIGridFive = "UIGridFive"; 

        // 那么就涉及游戏界面的折分：哪些是可以公用，哪些是不得不细化最小粒度的？
        
        public const string UIClassic = "UIClassic"; 

        public const string UIChallenge = "UIChallenge"; 
 // 挑战难度：要定义接口来实现20-50个不同的实现了？        
    }
}