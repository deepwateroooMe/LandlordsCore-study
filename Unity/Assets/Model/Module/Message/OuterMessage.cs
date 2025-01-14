// nerated by the protocol buffer compiler.  DO NOT EDIT!
// source: OuterMessage.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code
using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace ETModel {
#region Enums
    // 花色
    public enum Suits {
        // 梅花
        Club = 0,
        // 方块
        Diamond = 1,
        // 红心
        Heart = 2,
        // 黑桃
        Spade = 3,
        None = 4,
    }
    // 权重
    public enum Weight {
        // 3
        Three = 0,
        // 4
        Four = 1,
        // 5
        Five = 2,
        // 6
        Six = 3,
        // 7
        Seven = 4,
        // 8
        Eight = 5,
        // 9
        Nine = 6,
        // 10
        Ten = 7,
        // J
        Jack = 8,
        // Q
        Queen = 9,
        // K
        King = 10,
        // A
        One = 11,
        // 2
        Two = 12,
        // 小王
        Sjoker = 13,
        // 大王
        Ljoker = 14,
    }
    // 身份
    public enum Identity {
        None = 0,
        // 平民
        Farmer = 1,
        // 地主
        Landlord = 2,
    }
#endregion

#region Messages
    public partial class Actor_Test : pb::IMessage {
        private static readonly pb::MessageParser<Actor_Test> _parser = new pb::MessageParser<Actor_Test>(() => (Actor_Test)MessagePool.Instance.Fetch(typeof(Actor_Test)));
        public static pb::MessageParser<Actor_Test> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private string info_ = "";
        public string Info {
            get { return info_; }
            set {
                info_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (Info.Length != 0) {
                output.WriteRawTag(10);
                output.WriteString(Info);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            if (Info.Length != 0) {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Info);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            info_ = "";
            rpcId_ = 0;
            actorId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 10: {
                    Info = input.ReadString();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class Actor_TestRequest : pb::IMessage {
        private static readonly pb::MessageParser<Actor_TestRequest> _parser = new pb::MessageParser<Actor_TestRequest>(() => (Actor_TestRequest)MessagePool.Instance.Fetch(typeof(Actor_TestRequest)));
        public static pb::MessageParser<Actor_TestRequest> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private string request_ = "";
        public string Request {
            get { return request_; }
            set {
                request_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (Request.Length != 0) {
                output.WriteRawTag(10);
                output.WriteString(Request);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            if (Request.Length != 0) {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Request);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            request_ = "";
            rpcId_ = 0;
            actorId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 10: {
                    Request = input.ReadString();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class Actor_TestResponse : pb::IMessage {
        private static readonly pb::MessageParser<Actor_TestResponse> _parser = new pb::MessageParser<Actor_TestResponse>(() => (Actor_TestResponse)MessagePool.Instance.Fetch(typeof(Actor_TestResponse)));
        public static pb::MessageParser<Actor_TestResponse> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private int error_;
        public int Error {
            get { return error_; }
            set {
                error_ = value;
            }
        }
        private string message_ = "";
        public string Message {
            get { return message_; }
            set {
                message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        private string response_ = "";
        public string Response {
            get { return response_; }
            set {
                response_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (Response.Length != 0) {
                output.WriteRawTag(10);
                output.WriteString(Response);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (Error != 0) {
                output.WriteRawTag(216, 5);
                output.WriteInt32(Error);
            }
            if (Message.Length != 0) {
                output.WriteRawTag(226, 5);
                output.WriteString(Message);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Error != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(Error);
            }
            if (Message.Length != 0) {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(Message);
            }
            if (Response.Length != 0) {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Response);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            response_ = "";
            rpcId_ = 0;
            error_ = 0;
            message_ = "";
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 10: {
                    Response = input.ReadString();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 728: {
                    Error = input.ReadInt32();
                    break;
                }
                case 738: {
                    Message = input.ReadString();
                    break;
                }
                }
            }
        }
    }
    public partial class Actor_TransferRequest : pb::IMessage {
        private static readonly pb::MessageParser<Actor_TransferRequest> _parser = new pb::MessageParser<Actor_TransferRequest>(() => (Actor_TransferRequest)MessagePool.Instance.Fetch(typeof(Actor_TransferRequest)));
        public static pb::MessageParser<Actor_TransferRequest> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private int mapIndex_;
        public int MapIndex {
            get { return mapIndex_; }
            set {
                mapIndex_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (MapIndex != 0) {
                output.WriteRawTag(8);
                output.WriteInt32(MapIndex);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            if (MapIndex != 0) {
                size += 1 + pb::CodedOutputStream.ComputeInt32Size(MapIndex);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            mapIndex_ = 0;
            rpcId_ = 0;
            actorId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 8: {
                    MapIndex = input.ReadInt32();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class Actor_TransferResponse : pb::IMessage {
        private static readonly pb::MessageParser<Actor_TransferResponse> _parser = new pb::MessageParser<Actor_TransferResponse>(() => (Actor_TransferResponse)MessagePool.Instance.Fetch(typeof(Actor_TransferResponse)));
        public static pb::MessageParser<Actor_TransferResponse> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private int error_;
        public int Error {
            get { return error_; }
            set {
                error_ = value;
            }
        }
        private string message_ = "";
        public string Message {
            get { return message_; }
            set {
                message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (Error != 0) {
                output.WriteRawTag(216, 5);
                output.WriteInt32(Error);
            }
            if (Message.Length != 0) {
                output.WriteRawTag(226, 5);
                output.WriteString(Message);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Error != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(Error);
            }
            if (Message.Length != 0) {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(Message);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            rpcId_ = 0;
            error_ = 0;
            message_ = "";
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 728: {
                    Error = input.ReadInt32();
                    break;
                }
                case 738: {
                    Message = input.ReadString();
                    break;
                }
                }
            }
        }
    }
    public partial class C2G_EnterMap : pb::IMessage {
        private static readonly pb::MessageParser<C2G_EnterMap> _parser = new pb::MessageParser<C2G_EnterMap>(() => (C2G_EnterMap)MessagePool.Instance.Fetch(typeof(C2G_EnterMap)));
        public static pb::MessageParser<C2G_EnterMap> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            rpcId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                }
            }
        }
    }
    public partial class G2C_EnterMap : pb::IMessage {
        private static readonly pb::MessageParser<G2C_EnterMap> _parser = new pb::MessageParser<G2C_EnterMap>(() => (G2C_EnterMap)MessagePool.Instance.Fetch(typeof(G2C_EnterMap)));
        public static pb::MessageParser<G2C_EnterMap> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private int error_;
        public int Error {
            get { return error_; }
            set {
                error_ = value;
            }
        }
        private string message_ = "";
        public string Message {
            get { return message_; }
            set {
                message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        private long unitId_;
        // 自己的unit id
        public long UnitId {
            get { return unitId_; }
            set {
                unitId_ = value;
            }
        }
        private static readonly pb::FieldCodec<global::ETModel.UnitInfo> _repeated_units_codec
        = pb::FieldCodec.ForMessage(18, global::ETModel.UnitInfo.Parser);
        private pbc::RepeatedField<global::ETModel.UnitInfo> units_ = new pbc::RepeatedField<global::ETModel.UnitInfo>();
        // 所有的unit
        public pbc::RepeatedField<global::ETModel.UnitInfo> Units {
            get { return units_; }
            set { units_ = value; }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (UnitId != 0L) {
                output.WriteRawTag(8);
                output.WriteInt64(UnitId);
            }
            units_.WriteTo(output, _repeated_units_codec);
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (Error != 0) {
                output.WriteRawTag(216, 5);
                output.WriteInt32(Error);
            }
            if (Message.Length != 0) {
                output.WriteRawTag(226, 5);
                output.WriteString(Message);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Error != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(Error);
            }
            if (Message.Length != 0) {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(Message);
            }
            if (UnitId != 0L) {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(UnitId);
            }
            size += units_.CalculateSize(_repeated_units_codec);
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            unitId_ = 0;
            for (int i = 0; i < units_.Count; i++) { MessagePool.Instance.Recycle(units_[i]); }
            units_.Clear();
            rpcId_ = 0;
            error_ = 0;
            message_ = "";
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 8: {
                    UnitId = input.ReadInt64();
                    break;
                }
                case 18: {
                    units_.AddEntriesFrom(input, _repeated_units_codec);
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 728: {
                    Error = input.ReadInt32();
                    break;
                }
                case 738: {
                    Message = input.ReadString();
                    break;
                }
                }
            }
        }
    }
    public partial class UnitInfo : pb::IMessage {
        private static readonly pb::MessageParser<UnitInfo> _parser = new pb::MessageParser<UnitInfo>(() => (UnitInfo)MessagePool.Instance.Fetch(typeof(UnitInfo)));
        public static pb::MessageParser<UnitInfo> Parser { get { return _parser; } }
        private long unitId_;
        public long UnitId {
            get { return unitId_; }
            set {
                unitId_ = value;
            }
        }
        private float x_;
        public float X {
            get { return x_; }
            set {
                x_ = value;
            }
        }
        private float y_;
        public float Y {
            get { return y_; }
            set {
                y_ = value;
            }
        }
        private float z_;
        public float Z {
            get { return z_; }
            set {
                z_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (UnitId != 0L) {
                output.WriteRawTag(8);
                output.WriteInt64(UnitId);
            }
            if (X != 0F) {
                output.WriteRawTag(21);
                output.WriteFloat(X);
            }
            if (Y != 0F) {
                output.WriteRawTag(29);
                output.WriteFloat(Y);
            }
            if (Z != 0F) {
                output.WriteRawTag(37);
                output.WriteFloat(Z);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (UnitId != 0L) {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(UnitId);
            }
            if (X != 0F) {
                size += 1 + 4;
            }
            if (Y != 0F) {
                size += 1 + 4;
            }
            if (Z != 0F) {
                size += 1 + 4;
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            unitId_ = 0;
            x_ = 0f;
            y_ = 0f;
            z_ = 0f;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 8: {
                    UnitId = input.ReadInt64();
                    break;
                }
                case 21: {
                    X = input.ReadFloat();
                    break;
                }
                case 29: {
                    Y = input.ReadFloat();
                    break;
                }
                case 37: {
                    Z = input.ReadFloat();
                    break;
                }
                }
            }
        }
    }
    public partial class Actor_CreateUnits : pb::IMessage {
        private static readonly pb::MessageParser<Actor_CreateUnits> _parser = new pb::MessageParser<Actor_CreateUnits>(() => (Actor_CreateUnits)MessagePool.Instance.Fetch(typeof(Actor_CreateUnits)));
        public static pb::MessageParser<Actor_CreateUnits> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private static readonly pb::FieldCodec<global::ETModel.UnitInfo> _repeated_units_codec
        = pb::FieldCodec.ForMessage(10, global::ETModel.UnitInfo.Parser);
        private pbc::RepeatedField<global::ETModel.UnitInfo> units_ = new pbc::RepeatedField<global::ETModel.UnitInfo>();
        public pbc::RepeatedField<global::ETModel.UnitInfo> Units {
            get { return units_; }
            set { units_ = value; }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            units_.WriteTo(output, _repeated_units_codec);
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            size += units_.CalculateSize(_repeated_units_codec);
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            for (int i = 0; i < units_.Count; i++) { MessagePool.Instance.Recycle(units_[i]); }
            units_.Clear();
            rpcId_ = 0;
            actorId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 10: {
                    units_.AddEntriesFrom(input, _repeated_units_codec);
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class Frame_ClickMap : pb::IMessage {
        private static readonly pb::MessageParser<Frame_ClickMap> _parser = new pb::MessageParser<Frame_ClickMap>(() => (Frame_ClickMap)MessagePool.Instance.Fetch(typeof(Frame_ClickMap)));
        public static pb::MessageParser<Frame_ClickMap> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private long id_;
        public long Id {
            get { return id_; }
            set {
                id_ = value;
            }
        }
        private float x_;
        public float X {
            get { return x_; }
            set {
                x_ = value;
            }
        }
        private float y_;
        public float Y {
            get { return y_; }
            set {
                y_ = value;
            }
        }
        private float z_;
        public float Z {
            get { return z_; }
            set {
                z_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (X != 0F) {
                output.WriteRawTag(13);
                output.WriteFloat(X);
            }
            if (Y != 0F) {
                output.WriteRawTag(21);
                output.WriteFloat(Y);
            }
            if (Z != 0F) {
                output.WriteRawTag(29);
                output.WriteFloat(Z);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
            if (Id != 0L) {
                output.WriteRawTag(240, 5);
                output.WriteInt64(Id);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            if (Id != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(Id);
            }
            if (X != 0F) {
                size += 1 + 4;
            }
            if (Y != 0F) {
                size += 1 + 4;
            }
            if (Z != 0F) {
                size += 1 + 4;
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            x_ = 0f;
            y_ = 0f;
            z_ = 0f;
            rpcId_ = 0;
            actorId_ = 0;
            id_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 13: {
                    X = input.ReadFloat();
                    break;
                }
                case 21: {
                    Y = input.ReadFloat();
                    break;
                }
                case 29: {
                    Z = input.ReadFloat();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                case 752: {
                    Id = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class M2C_PathfindingResult : pb::IMessage {
        private static readonly pb::MessageParser<M2C_PathfindingResult> _parser = new pb::MessageParser<M2C_PathfindingResult>(() => (M2C_PathfindingResult)MessagePool.Instance.Fetch(typeof(M2C_PathfindingResult)));
        public static pb::MessageParser<M2C_PathfindingResult> Parser { get { return _parser; } }
        private long actorId_;
        public long ActorId {
            get { return actorId_; }
            set {
                actorId_ = value;
            }
        }
        private long id_;
        public long Id {
            get { return id_; }
            set {
                id_ = value;
            }
        }
        private float x_;
        public float X {
            get { return x_; }
            set {
                x_ = value;
            }
        }
        private float y_;
        public float Y {
            get { return y_; }
            set {
                y_ = value;
            }
        }
        private float z_;
        public float Z {
            get { return z_; }
            set {
                z_ = value;
            }
        }
        private static readonly pb::FieldCodec<float> _repeated_xs_codec
        = pb::FieldCodec.ForFloat(42);
        private pbc::RepeatedField<float> xs_ = new pbc::RepeatedField<float>();
        public pbc::RepeatedField<float> Xs {
            get { return xs_; }
            set { xs_ = value; }
        }
        private static readonly pb::FieldCodec<float> _repeated_ys_codec
        = pb::FieldCodec.ForFloat(50);
        private pbc::RepeatedField<float> ys_ = new pbc::RepeatedField<float>();
        public pbc::RepeatedField<float> Ys {
            get { return ys_; }
            set { ys_ = value; }
        }
        private static readonly pb::FieldCodec<float> _repeated_zs_codec
        = pb::FieldCodec.ForFloat(58);
        private pbc::RepeatedField<float> zs_ = new pbc::RepeatedField<float>();
        public pbc::RepeatedField<float> Zs {
            get { return zs_; }
            set { zs_ = value; }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (Id != 0L) {
                output.WriteRawTag(8);
                output.WriteInt64(Id);
            }
            if (X != 0F) {
                output.WriteRawTag(21);
                output.WriteFloat(X);
            }
            if (Y != 0F) {
                output.WriteRawTag(29);
                output.WriteFloat(Y);
            }
            if (Z != 0F) {
                output.WriteRawTag(37);
                output.WriteFloat(Z);
            }
            xs_.WriteTo(output, _repeated_xs_codec);
            ys_.WriteTo(output, _repeated_ys_codec);
            zs_.WriteTo(output, _repeated_zs_codec);
            if (ActorId != 0L) {
                output.WriteRawTag(232, 5);
                output.WriteInt64(ActorId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (ActorId != 0L) {
                size += 2 + pb::CodedOutputStream.ComputeInt64Size(ActorId);
            }
            if (Id != 0L) {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(Id);
            }
            if (X != 0F) {
                size += 1 + 4;
            }
            if (Y != 0F) {
                size += 1 + 4;
            }
            if (Z != 0F) {
                size += 1 + 4;
            }
            size += xs_.CalculateSize(_repeated_xs_codec);
            size += ys_.CalculateSize(_repeated_ys_codec);
            size += zs_.CalculateSize(_repeated_zs_codec);
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            id_ = 0;
            x_ = 0f;
            y_ = 0f;
            z_ = 0f;
            xs_.Clear();
            ys_.Clear();
            zs_.Clear();
            actorId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 8: {
                    Id = input.ReadInt64();
                    break;
                }
                case 21: {
                    X = input.ReadFloat();
                    break;
                }
                case 29: {
                    Y = input.ReadFloat();
                    break;
                }
                case 37: {
                    Z = input.ReadFloat();
                    break;
                }
                case 42:
                case 45: {
                    xs_.AddEntriesFrom(input, _repeated_xs_codec);
                    break;
                }
                case 50:
                case 53: {
                    ys_.AddEntriesFrom(input, _repeated_ys_codec);
                    break;
                }
                case 58:
                case 61: {
                    zs_.AddEntriesFrom(input, _repeated_zs_codec);
                    break;
                }
                case 744: {
                    ActorId = input.ReadInt64();
                    break;
                }
                }
            }
        }
    }
    public partial class C2R_Ping : pb::IMessage {
        private static readonly pb::MessageParser<C2R_Ping> _parser = new pb::MessageParser<C2R_Ping>(() => (C2R_Ping)MessagePool.Instance.Fetch(typeof(C2R_Ping)));
        public static pb::MessageParser<C2R_Ping> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            rpcId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                }
            }
        }
    }
    public partial class R2C_Ping : pb::IMessage {
        private static readonly pb::MessageParser<R2C_Ping> _parser = new pb::MessageParser<R2C_Ping>(() => (R2C_Ping)MessagePool.Instance.Fetch(typeof(R2C_Ping)));
        public static pb::MessageParser<R2C_Ping> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private int error_;
        public int Error {
            get { return error_; }
            set {
                error_ = value;
            }
        }
        private string message_ = "";
        public string Message {
            get { return message_; }
            set {
                message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (Error != 0) {
                output.WriteRawTag(216, 5);
                output.WriteInt32(Error);
            }
            if (Message.Length != 0) {
                output.WriteRawTag(226, 5);
                output.WriteString(Message);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Error != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(Error);
            }
            if (Message.Length != 0) {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(Message);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            rpcId_ = 0;
            error_ = 0;
            message_ = "";
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 728: {
                    Error = input.ReadInt32();
                    break;
                }
                case 738: {
                    Message = input.ReadString();
                    break;
                }
                }
            }
        }
    }
    public partial class G2C_Test : pb::IMessage {
        private static readonly pb::MessageParser<G2C_Test> _parser = new pb::MessageParser<G2C_Test>(() => (G2C_Test)MessagePool.Instance.Fetch(typeof(G2C_Test)));
        public static pb::MessageParser<G2C_Test> Parser { get { return _parser; } }
        public void WriteTo(pb::CodedOutputStream output) {}
        public int CalculateSize() {
            int size = 0;
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                }
            }
        }
    }
    public partial class C2M_Reload : pb::IMessage {
        private static readonly pb::MessageParser<C2M_Reload> _parser = new pb::MessageParser<C2M_Reload>(() => (C2M_Reload)MessagePool.Instance.Fetch(typeof(C2M_Reload)));
        public static pb::MessageParser<C2M_Reload> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private string account_ = "";
        public string Account {
            get { return account_; }
            set {
                account_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        private string password_ = "";
        public string Password {
            get { return password_; }
            set {
                password_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (Account.Length != 0) {
                output.WriteRawTag(10);
                output.WriteString(Account);
            }
            if (Password.Length != 0) {
                output.WriteRawTag(18);
                output.WriteString(Password);
            }
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Account.Length != 0) {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Account);
            }
            if (Password.Length != 0) {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Password);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            account_ = "";
            password_ = "";
            rpcId_ = 0;
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 10: {
                    Account = input.ReadString();
                    break;
                }
                case 18: {
                    Password = input.ReadString();
                    break;
                }
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                }
            }
        }
    }
    public partial class M2C_Reload : pb::IMessage {
        private static readonly pb::MessageParser<M2C_Reload> _parser = new pb::MessageParser<M2C_Reload>(() => (M2C_Reload)MessagePool.Instance.Fetch(typeof(M2C_Reload)));
        public static pb::MessageParser<M2C_Reload> Parser { get { return _parser; } }
        private int rpcId_;
        public int RpcId {
            get { return rpcId_; }
            set {
                rpcId_ = value;
            }
        }
        private int error_;
        public int Error {
            get { return error_; }
            set {
                error_ = value;
            }
        }
        private string message_ = "";
        public string Message {
            get { return message_; }
            set {
                message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (RpcId != 0) {
                output.WriteRawTag(208, 5);
                output.WriteInt32(RpcId);
            }
            if (Error != 0) {
                output.WriteRawTag(216, 5);
                output.WriteInt32(Error);
            }
            if (Message.Length != 0) {
                output.WriteRawTag(226, 5);
                output.WriteString(Message);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (RpcId != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(RpcId);
            }
            if (Error != 0) {
                size += 2 + pb::CodedOutputStream.ComputeInt32Size(Error);
            }
            if (Message.Length != 0) {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(Message);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            rpcId_ = 0;
            error_ = 0;
            message_ = "";
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 720: {
                    RpcId = input.ReadInt32();
                    break;
                }
                case 728: {
                    Error = input.ReadInt32();
                    break;
                }
                case 738: {
                    Message = input.ReadString();
                    break;
                }
                }
            }
        }
    }
    public partial class Card : pb::IMessage {
        private static readonly pb::MessageParser<Card> _parser = new pb::MessageParser<Card>(() => (Card)MessagePool.Instance.Fetch(typeof(Card)));
        public static pb::MessageParser<Card> Parser { get { return _parser; } }
        private global::ETModel.Weight cardWeight_ = 0;
        public global::ETModel.Weight CardWeight {
            get { return cardWeight_; }
            set {
                cardWeight_ = value;
            }
        }
        private global::ETModel.Suits cardSuits_ = 0;
        public global::ETModel.Suits CardSuits {
            get { return cardSuits_; }
            set {
                cardSuits_ = value;
            }
        }
        public void WriteTo(pb::CodedOutputStream output) {
            if (CardWeight != 0) {
                output.WriteRawTag(8);
                output.WriteEnum((int) CardWeight);
            }
            if (CardSuits != 0) {
                output.WriteRawTag(16);
                output.WriteEnum((int) CardSuits);
            }
        }
        public int CalculateSize() {
            int size = 0;
            if (CardWeight != 0) {
                size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) CardWeight);
            }
            if (CardSuits != 0) {
                size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) CardSuits);
            }
            return size;
        }
        public void MergeFrom(pb::CodedInputStream input) {
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch(tag) {
                default:
                    input.SkipLastField();
                    break;
                case 8: {
                    cardWeight_ = (global::ETModel.Weight) input.ReadEnum();
                    break;
                }
                case 16: {
                    cardSuits_ = (global::ETModel.Suits) input.ReadEnum();
                    break;
                }
                }
            }
        }
    }
#endregion
}
#endregion Designer generated code
