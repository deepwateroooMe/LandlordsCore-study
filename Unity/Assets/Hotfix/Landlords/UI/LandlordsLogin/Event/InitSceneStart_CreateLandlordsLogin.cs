using ETModel;
namespace ETHotfix {

 // 加载的时候，扫描到的标签系统，这个标签，就对应了这么个事件    
    [Event(EventIdType.LandlordsInitSceneStart)]
    public class InitSceneStart_CreateLandlordsLogin : AEvent {
        public override void Run() {
            // 创建登录界面
            UI ui = Game.Scene.GetComponent<UIComponent>().Create(UIType.LandlordsLogin);
        }
    }
}
