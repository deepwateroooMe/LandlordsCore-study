using System.Threading.Tasks;
namespace ETModel {
    
// 结构体：把它定义为，帮助定位【 xx 】的位置？将把位置消息返回回来
    public struct ActorTask {

        public IActorRequest ActorRequest;
        public TaskCompletionSource<IActorLocationResponse> Tcs;

        public ActorTask(IActorLocationMessage actorRequest) { // 只发完，不要返回消息，发了会起什么作用呢？
            this.ActorRequest = actorRequest;
            this.Tcs = null;
        }
        public ActorTask(IActorLocationRequest actorRequest, TaskCompletionSource<IActorLocationResponse> tcs) { // 需要返回位置消息
            this.ActorRequest = actorRequest;
            this.Tcs = tcs;
        }
    }
}