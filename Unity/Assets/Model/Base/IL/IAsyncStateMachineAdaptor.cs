using System;
using System.Runtime.CompilerServices;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ETModel {

    // 协程适配器？或是如Collections.List IIterator 的适配器？名字起得狠强大，但仍是普通Iterator 或是协程适配器呀
    [ILAdapter]
    public class IAsyncStateMachineClassInheritanceAdaptor : CrossBindingAdaptor { 
        public override Type BaseCLRType {
            get {
                return typeof (IAsyncStateMachine);
            }
        }
        public override Type AdaptorType {
            get {
                return typeof (IAsyncStateMachineAdaptor);
            }
        }
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            return new IAsyncStateMachineAdaptor(appdomain, instance);
        }
        
        public class IAsyncStateMachineAdaptor: IAsyncStateMachine, CrossBindingAdaptorType {

            private ILTypeInstance instance;
            private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
            private IMethod mMoveNext;
            private IMethod mSetStateMachine;
            private readonly object[] param1 = new object[1]; // 这个是为减少GC 的【这里好像并没有真正用到它，找个真正使用它的地方，再看一下】

            public IAsyncStateMachineAdaptor() {
            }
            public IAsyncStateMachineAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance) {
                this.appDomain = appDomain;
                this.instance = instance;
            }
            public ILTypeInstance ILInstance {
                get {
                    return instance;
                }
            }

            public void MoveNext() {
                if (this.mMoveNext == null) {
                    mMoveNext = instance.Type.GetMethod("MoveNext", 0);
                }
                this.appDomain.Invoke(mMoveNext, instance, null);
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine) {
                if (this.mSetStateMachine == null) {
                    mSetStateMachine = instance.Type.GetMethod("SetStateMachine");
                }
                this.appDomain.Invoke(mSetStateMachine, instance, stateMachine);
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
