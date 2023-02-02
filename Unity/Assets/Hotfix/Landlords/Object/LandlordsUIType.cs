namespace ETHotfix {

// 热更新域里面：  定义了这么几个UI相关的类型
    public static partial class UIType {
        public const string LandlordsLogin = "LandlordsLogin";
        public const string LandlordsLobby = "LandlordsLobby";　 // 主菜单　三选项

 // 这里需要三种不同模式下的视图：所以需要添加几个相关的UI组件 ？
         // Educational：有个过度的，这里UI的装配有点儿复杂
         // Classic：一个视图界面
         // Challenge：它相对独立，可以适用的场景比较少
        
        public const string LandlordsEnd = "LandlordsEnd";
 
// 不打算要下面的 两行。为维护项目的可运行性，暂时留着
        public const string LandlordsRoom = "LandlordsRoom";
        public const string LandlordsInteraction = "LandlordsInteraction";
    }
}
