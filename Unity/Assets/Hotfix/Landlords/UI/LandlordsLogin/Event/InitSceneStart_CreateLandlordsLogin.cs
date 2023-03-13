using ETModel;
namespace ETHotfix {

// EventAttribute 说，这是事件类型标签属性。那么事件是如何触发的呢？只要程序逻辑里触发或是调用了某标签系标明的事件，这些所有标签标明了这个同一Event 类型的类里的方法回调，都会一一调用？
// 加载的时候，扫描到的Event标签系统。当ETHotFix.Init() 里调用了这个事件，事件标签所标明的这个事件类型的类，当前类，就对应了这么个事件，会调用 Run() 方法 
    [Event(EventIdType.LandlordsInitSceneStart)]
    public class InitSceneStart_CreateLandlordsLogin : AEvent {
        public override void Run() { // 那么就是说，程序逻辑调用或是触发了，此Event, 这一同类同标签为这一类型的所有事件回调类都会一一调用 
            // 创建登录界面
            UI ui = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsLogin);
        }
    }
}
