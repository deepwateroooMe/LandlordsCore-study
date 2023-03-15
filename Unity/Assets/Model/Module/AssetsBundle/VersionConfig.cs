using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel {

    // 文件版本信息
    public class FileVersionInfo {
        public string File; // 文件名
        public string MD5;  // <<<<<<<<<<<<<<<<<<<< MD5 码值 
        public long Size;   // 文件大小
    }
    // 是对服务器，或是客户端的所有资源包的管理：
    public class VersionConfig : Object {

        public int Version;
        public long TotalSize; // 所有资源包的总大小
        
        [BsonIgnore]
        public Dictionary<string, FileVersionInfo> FileInfoDict = new Dictionary<string, FileVersionInfo>();

        public override void EndInit() {
            base.EndInit();
            foreach (FileVersionInfo fileVersionInfo in this.FileInfoDict.Values) {
                this.TotalSize += fileVersionInfo.Size; // 所有资源包的总大小
            }
        }
    }
}