﻿ namespace ETModel {
    // 托管组件，逻辑在TrusteeshipComponentSystem扩展
    public class TrusteeshipComponent : Component {

        public bool Playing { get; set; }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.Playing = false;
        }
    }
}
