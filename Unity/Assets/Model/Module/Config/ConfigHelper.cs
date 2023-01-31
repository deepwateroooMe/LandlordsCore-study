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
                GameObject config = (GameObject)ResourcesHelper.Load("KV"); // 不明白，这里说的是什么意思  KV  。。。
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