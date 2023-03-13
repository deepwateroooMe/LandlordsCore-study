using System;
using ETModel;
using Google.Protobuf;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using IMessage = Google.Protobuf.IMessage;

namespace Google.Protobuf {

    [ILAdapter] // 使用自定义的标签：来标注，当前类的类型为，ILRuntime 热更新程序域的适配器
    public class Adapt_IMessage: CrossBindingAdaptor {
        public override Type BaseCLRType {
            get {
                return typeof (IMessage);
            }
        }
        public override Type AdaptorType {
            get {
                return typeof (Adaptor);
            }
        }
        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance) {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor: MyAdaptor, IMessage { // 它必须得实现 Google.Protobuf.IMessage 接口的三个方法，在新程序域里的定义实现逻辑
            public Adaptor(AppDomain appdomain, ILTypeInstance instance): base(appdomain, instance) {
            }
            protected override AdaptHelper.AdaptMethod[] GetAdaptMethods() {
                // 这个适配器里，注册和定义，适配了如下三个方法
                AdaptHelper.AdaptMethod[] methods = {
                    new AdaptHelper.AdaptMethod { Name = "MergeFrom", ParamCount = 1 },
                    new AdaptHelper.AdaptMethod { Name = "WriteTo", ParamCount = 1 },
                    new AdaptHelper.AdaptMethod { Name = "CalculateSize", ParamCount = 0 },
                };
                return methods;
            }
            public void MergeFrom(CodedInputStream input) {
                Invoke(0, input);
            }
            public void WriteTo(CodedOutputStream output) {
                Invoke(1, output);
            }
            public int CalculateSize() {
                return (int) Invoke(2);
            }
        }
    }
}