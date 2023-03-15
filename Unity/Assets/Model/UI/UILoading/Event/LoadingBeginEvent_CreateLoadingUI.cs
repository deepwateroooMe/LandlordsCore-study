namespace ETModel {
    // ET 框架系统标签 EventAttribute 所标记的事件类型 EventIdType.LoadingBegin, 被调用时【比如：BundleHelper 类中对此事件的调用】，会调用下面定义的 Run() 方法 
    [Event(EventIdType.LoadingBegin)]
    public class LoadingBeginEvent_CreateLoadingUI : AEvent {

        public override void Run() {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UILoading);
        }
    }
}
