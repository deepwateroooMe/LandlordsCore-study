using System;
using System.IO;

namespace ETModel {
    public class SessionCallbackComponent: Component { // 这就是前面提到过，有的客户端，有些情况下，是需要囘调的，ETModel-Session 里会逻辑处理回调回来

        public Action<Session, byte, ushort, MemoryStream> MessageCallback;
        public Action<Session> DisposeCallback;

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.DisposeCallback?.Invoke(this.GetParent<Session>());
        }
    }
}
