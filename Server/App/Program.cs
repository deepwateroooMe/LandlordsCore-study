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
            // 异步方法全部会回掉到主线程
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
                // 根据不同的AppType添加不同的组件: 
                OuterConfig outerConfig = startConfig.GetComponent<OuterConfig>();
                InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
                ClientConfig clientConfig = startConfig.GetComponent<ClientConfig>();
                
                switch (startConfig.AppType) { // 根据服务器的类型，来添加相关类型所需要的组件
                case AppType.Manager:
                    Game.Scene.AddComponent<AppManagerComponent>();
                    Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                    Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                    break;
                case AppType.Realm: // 现在看到的，绝大多数的登录服都是用的这个
                    Game.Scene.AddComponent<ActorMessageDispatherComponent>();
// 内网网络组件NetInnerComponent，处理对内网连接： 就是不同服务器之间（登录服，Gate服，Map服，Locatioin服等等）
                    Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
// 外网网络组件NetOuterComponent，处理与客户端连接（与客户端有交互的，主要是登录服  与 Gate服，登录后就主要通过gate服中转了）
                    Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                    Game.Scene.AddComponent<LocationProxyComponent>();
                    Game.Scene.AddComponent<RealmGateAddressComponent>();
                    break;
                case AppType.Gate:
                    Game.Scene.AddComponent<PlayerComponent>();
                    Game.Scene.AddComponent<ActorMessageDispatherComponent>();
                    Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                    Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
                    Game.Scene.AddComponent<LocationProxyComponent>();
                    Game.Scene.AddComponent<ActorMessageSenderComponent>();
                    Game.Scene.AddComponent<ActorLocationSenderComponent>();
                    Game.Scene.AddComponent<GateSessionKeyComponent>();
                    break;
                case AppType.Location:
                    Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                    Game.Scene.AddComponent<LocationComponent>();
                    break;
                case AppType.Map:
                    Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
                    Game.Scene.AddComponent<UnitComponent>();
                    Game.Scene.AddComponent<LocationProxyComponent>();
                    Game.Scene.AddComponent<ActorMessageSenderComponent>();
                    Game.Scene.AddComponent<ActorLocationSenderComponent>();
                    Game.Scene.AddComponent<ActorMessageDispatherComponent>();
                    Game.Scene.AddComponent<PathfindingComponent>();
                    break;
                case AppType.AllServer: // <<<<<<<<<<<<<<<<<<<< 我先重点就看这个，好像还是由8个项目构成的
                    Game.Scene.AddComponent<ActorMessageSenderComponent>();
                    Game.Scene.AddComponent<ActorLocationSenderComponent>();
                    Game.Scene.AddComponent<PlayerComponent>();
                    Game.Scene.AddComponent<UnitComponent>();
                    // PS：如果启动闪退有可能是服务器配置文件没有填数据库配置，请正确填写
                    // 这里需要将DBComponent的Awake注释去掉才能连接MongoDB
                    Game.Scene.AddComponent<DBComponent>(); // 这个，就成为服务器端的一个重点，但是是最简单的重点，因为相比其它，它最容易
                    // 这里需要加上DBCacheComponent才能操作MongoDB
                    Game.Scene.AddComponent<DBCacheComponent>();
                    Game.Scene.AddComponent<DBProxyComponent>();
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
