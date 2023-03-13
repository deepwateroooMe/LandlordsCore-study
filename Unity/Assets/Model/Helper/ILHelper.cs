using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
namespace ETModel {
    public static class ILHelper { // 这里看得不透

        public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain) { // 同自己的ILRuntime 热更新程序域的初始化配置相比，简化了狠多，也复杂了狠多
            // 注册重定向函数
            // 注册委托
            appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appdomain.DelegateManager.RegisterMethodDelegate<AChannel, System.Net.Sockets.SocketError>();
            appdomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<IResponse>();
            appdomain.DelegateManager.RegisterMethodDelegate<Session, object>();
            appdomain.DelegateManager.RegisterMethodDelegate<Session, byte, ushort, MemoryStream>();
            appdomain.DelegateManager.RegisterMethodDelegate<Session>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterMethodDelegate<Google.Protobuf.Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.Adapt_IMessage.Adaptor, System.Boolean>();
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<Google.Protobuf.Adapt_IMessage.Adaptor>>((act) => {
                return new System.Predicate<Google.Protobuf.Adapt_IMessage.Adaptor>((obj) => {
                    return ((Func<Google.Protobuf.Adapt_IMessage.Adaptor, System.Boolean>)act)(obj);
                });
            });
            CLRBindings.Initialize(appdomain);
            // 注册适配器
            Assembly assembly = typeof(Init).Assembly;
            foreach (Type type in assembly.GetTypes()) {
// 这里同样使用标签系：ET 框架中定义了三类适配器： google-Protobuf-IMessage, IDisposable, 和 Iterator 协程适配器？
// 这里的问题是：似乎这个双端框架中，还没能明白，哪些程序域是必须得适配的？当然是热更新程序域，双端的这些热更新程序域。就是双端的 Hotfix 里必须得适配的 ? 需要想一想这个过程
                //Protobuf 中的 IMessage 连接双端，就已经是跨域了呀，服务器端的程序域是服务器端的，客户端是客户端的
                object[] attrs = type.GetCustomAttributes(typeof(ILAdapterAttribute), false); // 框架中有三类适配器 
                if (attrs.Length == 0) {
                    continue;
                }
                object obj = Activator.CreateInstance(type);
                CrossBindingAdaptor adaptor = obj as CrossBindingAdaptor;
                if (adaptor == null) {
                    continue;
                }
                appdomain.RegisterCrossBindingAdaptor(adaptor);
            }
// 这里也是糊涂的：为什么需要如自己项目中的AddComponent<HotfixTypeT>() GetComponent<HotfixTypeT> 一样对Json 相关的方法重定向？
// 类比来想的话，像是申明，热更新程序域中的某些Json Object 类型，LitJson 要使用的时候，使用JsonMapper 重定向到。。。？
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain); 
        }
    }
}