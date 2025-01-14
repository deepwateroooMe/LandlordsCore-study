﻿namespace ETModel {

    public static class SceneType { // 这里，好像是，客户端自定义的游戏场景呀；服务器端对应的服务器场景是 Scene
        public const string Share = "Share";
        public const string Game = "Game";
        public const string Login = "Login";
        public const string Lobby = "Lobby"; // 这里，游戏大厅，也当作一个场景了
        public const string Map = "Map";
        public const string Launcher = "Launcher";
        public const string Robot = "Robot";
        public const string RobotClient = "RobotClient";
        public const string Realm = "Realm";

        public const string Account = "Account"; // 加了这个，不知道对不对？
    }
    
    public sealed class Scene: Entity {
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
        }
    }
}