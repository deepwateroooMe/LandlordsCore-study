 using ETModel;
namespace ETHotfix {

    public static class GateHelper {

        // 验证Session是否合法
        public static bool SignSession(Session session) {
            SessionUserComponent sessionUser = session.GetComponent<SessionUserComponent>();
            if (sessionUser == null || Game.Scene.GetComponent<UserComponent>().Get(sessionUser.User.UserID) == null) {
                return false;
            }
            return true;
        }
    }
}
