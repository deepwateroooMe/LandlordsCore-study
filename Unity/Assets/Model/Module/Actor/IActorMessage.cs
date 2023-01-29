namespace ETModel {

    // 不需要返回消息: 发完了就可以了
    public interface IActorMessage: IMessage {
        long ActorId { get; set; }
    }
    public interface IActorRequest : IRequest { // 有要求，就要有返回，需要回复的
        long ActorId { get; set; }
    }
    public interface IActorResponse : IResponse {
    }

    public interface IFrameMessage : IMessage { // 要求桢同步
        long Id { get; set; }
    }
}