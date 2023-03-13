using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ETModel;
using NLog;
using PF;
using ABPath = ETModel.ABPath;
using Path = System.IO.Path;
namespace App {
    internal static class Program { // 服务器端的入口程序

        private static void Main(string[] args) {
            // 异步方法全部会回掉到主线程: 当前这个线程就是主线程
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
            
            try {            
                Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
                Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());
                Options options = Game.Scene.AddComponent<OptionComponent, string[]>(args).Options;
                
// 下面的：都是从应用的配置里读取出来的
                StartConfig startConfig = Game.Scene.AddComponent<StartConfigComponent, string, int>(options.Config, options.AppId).StartConfig; 
                if (!options.AppType.Is(startConfig.AppType)) {
                    Log.Error("命令行参数apptype与配置不一致");
                    return;
                }
                IdGenerater.AppId = options.AppId;
                LogManager.Configuration.Variables["appType"] = startConfig.AppType.ToString();
                LogManager.Configuration.Variables["appId"] = startConfig.AppId.ToString();
                LogManager.Configuration.Variables["appTypeFormat"] = $"{startConfig.AppType,-8}";
                LogManager.Configuration.Variables["appIdFormat"] = $"{startConfig.AppId:D3}";
                Log.Info($"server start........................ {startConfig.AppId} {startConfig.AppType}");
// 加載服务器端的相关的组件
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                Game.Scene.AddComponent<MessageDispatherComponent>(); // 消息分发器
                OuterConfig outerConfig = startConfig.GetComponent<OuterConfig>();
                InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
                ClientConfig clientConfig = startConfig.GetComponent<ClientConfig>();
// 根据不同的AppType添加不同的组件: 
                switch (startConfig.AppType) { // 根据服务器的类型，来添加相关类型所需要的组件：由最顶层的管理架构，到相对底层的服务器组件
                    case AppType.Manager:
                        Game.Scene.AddComponent<AppManagerComponent>(); // 对服务器的宕机管理：宕机，拉起保活
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                        Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                        break;
                    case AppType.Realm: // 注册登录服：
                        Game.Scene.AddComponent<ActorMessageDispatherComponent>(); // 要向网关 gate 服发消息
    // 内网网络组件NetInnerComponent，处理对内网连接： 就是不同服务器之间（登录服，Gate服，Map服，Locatioin服等等）
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
    // 外网网络组件NetOuterComponent，处理与客户端连接（与客户端有交互的，主要是登录服  与 Gate服，登录后就主要通过gate服中转了）
                        Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                        Game.Scene.AddComponent<LocationProxyComponent>(); // 找人找不到的时候，要它帮忙定位 ?
                        Game.Scene.AddComponent<RealmGateAddressComponent>(); // 主管：分配玩家到某个网关下的逻辑 ?
                        break;
                case AppType.Gate: // 网关服：可以有很多个
                    Game.Scene.AddComponent<PlayerComponent>(); // 玩家登录后，只与网关服交互。它要管理玩家
                        Game.Scene.AddComponent<ActorMessageDispatherComponent>();
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                        Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                        Game.Scene.AddComponent<LocationProxyComponent>(); // 代理是说，网关向代理索要位置位置，真正的定位？不明白
                        Game.Scene.AddComponent<ActorMessageSenderComponent>();
                        Game.Scene.AddComponent<ActorLocationSenderComponent>(); // 发定位消息，需要返回位置消息。定位玩家：主要用于，当前网关下的玩家，要给他从前的小伙伴发消息等，或是接收者换场景了等情况下
                        Game.Scene.AddComponent<GateSessionKeyComponent>(); // 管理，当前网关下，所有登录过的有效【20 秒后需要重新申请 key】玩家的认证，主要是管理玩家
                        break;
                case AppType.Location: // 位置服务器
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                        Game.Scene.AddComponent<LocationComponent>();
                        break;
                case AppType.Map: // 地图服务器：
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                        Game.Scene.AddComponent<UnitComponent>(); // 就把它理解为，玩家位置信息
                        Game.Scene.AddComponent<LocationProxyComponent>();
                        Game.Scene.AddComponent<ActorMessageSenderComponent>();
                        Game.Scene.AddComponent<ActorLocationSenderComponent>();
                        Game.Scene.AddComponent<ActorMessageDispatherComponent>();
                        Game.Scene.AddComponent<PathfindingComponent>(); // 这里面有个好大的PF:PathFinding 库，没看
                        break;
                    case AppType.AllServer: // <<<<<<<<<<<<<<<<<<<< 我先重点就看这个，好像还是由8个项目构成的
                        Game.Scene.AddComponent<ActorMessageSenderComponent>();
                        Game.Scene.AddComponent<ActorLocationSenderComponent>();
                        Game.Scene.AddComponent<PlayerComponent>(); //MyPlayer: 这个没明白
                        Game.Scene.AddComponent<UnitComponent>();
                        // PS：如果启动闪退有可能是服务器配置文件没有填数据库配置，请正确填写
                        // 这里需要将DBComponent的Awake注释去掉才能连接MongoDB
                        Game.Scene.AddComponent<DBComponent>(); // 数据库操作：主要是包装成异步任务
                        // 这里需要加上DBCacheComponent才能操作MongoDB
                        Game.Scene.AddComponent<DBCacheComponent>(); // 数据库缓存：
                        Game.Scene.AddComponent<DBProxyComponent>(); // 代理
                        Game.Scene.AddComponent<LocationComponent>();
                        Game.Scene.AddComponent<ActorMessageDispatherComponent>();
                        Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                        Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                        Game.Scene.AddComponent<LocationProxyComponent>();
                        Game.Scene.AddComponent<AppManagerComponent>();
                        Game.Scene.AddComponent<RealmGateAddressComponent>(); // <<<<<<<<<<<<<<<<<<<< 
                        Game.Scene.AddComponent<GateSessionKeyComponent>();
                        Game.Scene.AddComponent<ConfigComponent>();
                        // Game.Scene.AddComponent<ServerFrameComponent>();
                        Game.Scene.AddComponent<PathfindingComponent>();
                        // Game.Scene.AddComponent<HttpComponent>();

                        // 以下是斗地主服务端自定义全局组件
                        // GateGlobalComponent
                        Game.Scene.AddComponent<UserComponent>();
                        Game.Scene.AddComponent<LandlordsGateSessionKeyComponent>(); // <<<<<<<<<< 为什么这里要特制一个，同上面有什么不同？如果只是类名的不同，仅只为了客户端热更新方便吗？
                        // MapGlobalComponent
                        Game.Scene.AddComponent<RoomComponent>();
                        // MatchGlobalComponent
                        Game.Scene.AddComponent<AllotMapComponent>();
                        Game.Scene.AddComponent<MatchComponent>();
                        Game.Scene.AddComponent<MatcherComponent>();
                        Game.Scene.AddComponent<MatchRoomComponent>();
                        // RealmGlobalComponent
                        Game.Scene.AddComponent<OnlineComponent>();
                        break;
                    case AppType.Benchmark:
                        Game.Scene.AddComponent<NetOuterComponent>();
                        Game.Scene.AddComponent<BenchmarkComponent, string>(clientConfig.Address);
                        break;
                    case AppType.BenchmarkWebsocketServer:
                        Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                        break;
                    case AppType.BenchmarkWebsocketClient:
                        Game.Scene.AddComponent<NetOuterComponent>();
                        Game.Scene.AddComponent<WebSocketBenchmarkComponent, string>(clientConfig.Address);
                        break;
                    default:
                        throw new Exception($"命令行参数没有设置正确的AppType: {startConfig.AppType}");
                }
                
                while (true) {
                    try
                    {
                        Thread.Sleep(1);
                        OneThreadSynchronizationContext.Instance.Update();
                        Game.EventSystem.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
