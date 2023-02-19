namespace ETModel {

    // 主要是用于  定位服务器，帮助定位小伙伴是在哪里
    public interface IActorLocationMessage : IActorRequest {
    }
    public interface IActorLocationRequest : IActorRequest {
    }
    public interface IActorLocationResponse : IActorResponse {
    }
}