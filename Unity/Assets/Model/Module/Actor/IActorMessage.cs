namespace ETModel {

// 发消息：要知道对方地址
    public interface IActorMessage: IMessage { // 不需要返回消息: 发完了就可以了
        long ActorId { get; set; }
    }
    public interface IActorRequest : IRequest { // 有要求，就要有返回，需要回复的
        long ActorId { get; set; }
    }
    
// 返回消息：就不用地址了
    public interface IActorResponse : IResponse {
    }

    public interface IFrameMessage : IMessage { // 要求桢同步：找个栗子
        long Id { get; set; }
    }
}