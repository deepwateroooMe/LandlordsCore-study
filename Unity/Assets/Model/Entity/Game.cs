﻿namespace ETModel { // 这个域里面所定义的游戏类：应该是可以双端共享的
    public static class Game {
        // 静态类：规定了游戏的几大组件：场景，事件回调系统，对象池，热更新，关？
        private static Scene scene;
        public static Scene Scene {
            get {
                if (scene != null) {
                    return scene;
                }
                scene = new Scene();
                scene.AddComponent<TimerComponent>();
                return scene;
            }
        }

        private static EventSystem eventSystem;
        public static EventSystem EventSystem {
            get {
                return eventSystem ?? (eventSystem = new EventSystem());
            }
        }

        private static ObjectPool objectPool;
        public static ObjectPool ObjectPool {
            get {
                return objectPool ?? (objectPool = new ObjectPool());
            }
        }

        private static Hotfix hotfix;
        public static Hotfix Hotfix {
            get {
                return hotfix ?? (hotfix = new Hotfix());
            }
        }

        public static void Close() {
            scene.Dispose();
            eventSystem = null;
            scene = null;
            objectPool = null;
            hotfix = null;
        }
    }
}