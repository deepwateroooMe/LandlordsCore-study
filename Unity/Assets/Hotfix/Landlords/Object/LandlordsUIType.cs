namespace ETHotfix {

// 热更新域里面：  定义了这么几个UI相关的类型
    public static partial class UIType { // 就没有弄明白：这个部分类，与其它模块同名类，有什么区别，这里必须得改吗，会影响哪些功能 ？
 // 这个大概可以少定义一点儿，两个文件都加就会造成混淆。也就是说，这个类可能只需要部分传递，部分事件就可以了 ？？？
        
        public const string LandlordsLogin = "LandlordsLogin";
        public const string LandlordsLobby = "LandlordsLobby";　 // 主菜单　三选项
        
        public const string LandlordsEnd = "LandlordsEnd";

// 这里需要三种不同模式下的视图：所以需要添加几个相关的UI组件 ？
         // Educational：有个过度的，这里UI的装配有点儿复杂
         // Classic：一个视图界面
         // Challenge：它相对独立，可以适用的场景比较少

        // // 上面的界面远远不够呀。。。
        // public const string UIEducationalMode = "UIEducationalMode"; 

        // public const string UIEducational = "UIEducational"; 
        // // 怎么再把它细化为：三　四　五方格呢？ 应该是要用同一接口的不同实现，完全重复写三个系统会把人弄死的。。。。。
        // public const string UIGridThree = "UIGridThree";
        // public const string UIGridFour = "UIGridFour"; 
        // public const string UIGridFive = "UIGridFive"; 

        // // 那么就涉及游戏界面的折分：哪些是可以公用，哪些是不得不细化最小粒度的？
        
        // public const string UIClassic = "UIClassic"; 

        // public const string UIChallenge = "UIChallenge"; 
        // // 挑战难度：要定义接口来实现20-50个不同的实现了？        

        
// 不打算要下面的 两行。为维护项目的可运行性，暂时留着
        public const string LandlordsRoom = "LandlordsRoom";
        public const string LandlordsInteraction = "LandlordsInteraction";
    }
}
