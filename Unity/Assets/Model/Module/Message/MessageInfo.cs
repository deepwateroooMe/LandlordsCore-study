namespace ETModel {
    public struct MessageInfo {

        public ushort Opcode { get; } // 总是携带操作码：这个操作码双端必须公认，否则会出错
        public object Message { get; }

        public MessageInfo(ushort opcode, object message) {
            this.Opcode = opcode;
            this.Message = message;
        }
    }
}
