namespace ETHotfix {
    // 客户端的游戏场景，很简单，就几样：两个框架与游戏之间，总是弄混，这个是斗地主版本
    public static class Game {

        private static Scene scene;
        public static Scene Scene {
            get {
                if (scene != null) {
                    return scene;
                }
                scene = new Scene();
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

        public static void Close() {
            scene.Dispose();
            scene = null;
            eventSystem = null;
            objectPool = null;
        }
    }
}