using System;
using UnityEngine;

namespace ETModel {
    public static class ConfigHelper {

        public static string GetText(string key) {
            try {
                 // 读取  config.unity3d资源包中的Config这一项这个字符串预设
                GameObject config = (GameObject)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("config.unity3d", "Config");
                string configStr = config.Get<TextAsset>(key).text;
                return configStr;
            }
            catch (Exception e) {
                throw new Exception($"load config file fail, key: {key}", e);
            }
        }
        
        public static string GetGlobal() {
            try {
                GameObject config = (GameObject)ResourcesHelper.Load("KV"); // 这里应该，也是一个资源包的名字，叫KV
                string configStr = config.Get<TextAsset>("GlobalProto").text;
                return configStr;
            }
            catch (Exception e) {
                throw new Exception($"load global config file fail", e);
            }
        }
        public static T ToObject<T>(string str) {
            return JsonHelper.FromJson<T>(str);
        }
    }
}