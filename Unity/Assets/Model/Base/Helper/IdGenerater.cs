namespace ETModel {
    public static class IdGenerater {

        public static long AppId { private get; set; }
        private static ushort value;

        public static long GenerateId() {
            long time = TimeHelper.ClientNowSeconds();
            return (AppId << 48) + (time << 16) + ++value;
        }

        public static int GetAppIdFromId(long id) { // 这个 ID 生成器，设计得比较聪明一点儿，移位就可以知道应用地址
            return (int)(id >> 48);
        }
    }
}