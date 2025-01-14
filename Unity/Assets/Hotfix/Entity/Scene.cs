﻿namespace ETHotfix {
    public sealed class Scene: Entity {

        public ETModel.Scene ModelScene { get; set; } = new ETModel.Scene();
        public string Name { get; set; }

        public Scene() {
        }
        public Scene(long id): base(id) {
        }

        public override void Dispose() {
            if (this.IsDisposed) {
                return;
            }
            base.Dispose();
            this.ModelScene.Dispose();
        }
    }
}