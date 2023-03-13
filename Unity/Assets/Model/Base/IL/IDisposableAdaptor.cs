using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ETModel {

    [ILAdapter] // 定义的是所有继承自 IDisposable 接口的实现类的跨域适配：主要是定义接口类中的 Dispose() 方法在新程序域里的实现逻辑
    public class IDisposableClassInheritanceAdaptor : CrossBindingAdaptor {
        public override Type BaseCLRType {
            get {
                return typeof (IDisposable);
            }
        }
        public override Type AdaptorType {
            get {
                return typeof (IDisposableAdaptor);
            }
        }
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            return new IDisposableAdaptor(appdomain, instance);
        }

        public class IDisposableAdaptor: IDisposable, CrossBindingAdaptorType {
            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
            
            private IMethod iDisposable;
            private readonly object[] param0 = new object[0]; // 

            public IDisposableAdaptor() {
            }
            public IDisposableAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance) {
                this.appDomain = appDomain;
                this.instance = instance;
            }
            public ILTypeInstance ILInstance {
                get {
                    return instance;
                }
            }

            public void Dispose() { // 主要是定义这个接口方法在新的程序域里的实现
                if (this.iDisposable == null) {
                    this.iDisposable = instance.Type.GetMethod("Dispose");
                }
                this.appDomain.Invoke(this.iDisposable, instance, this.param0);
            }

            public override string ToString() {
                IMethod m = this.appDomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod) {
                    return instance.ToString();
                }
                return instance.Type.FullName;
            }
        }
    }
}
