﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ETModel {
    public sealed class Hotfix : Object {

#if ILRuntime
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
#else
        private Assembly assembly;
#endif
        private IStaticMethod start;
        public Action Update; // 程序域层面，必备的几个回调事件，与游戏引擎中的回调事件同步
        public Action LateUpdate;
        public Action OnApplicationQuit;

        public Hotfix() {  }

        public void GotoHotfix() {
#if ILRuntime
            ILHelper.InitILRuntime(this.appDomain); // 这个方法里作：ILRuntime热更新的必要的加载
#endif
            this.start.Run(); // <<<<<<<<<<<<<<<<<<<< 
        }

        public List<Type> GetHotfixTypes() {
#if ILRuntime
            if (this.appDomain == null) {
                return new List<Type>();
            }
            return this.appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
#else
            if (this.assembly == null) {
                return new List<Type>();
            }
            return this.assembly.GetTypes().ToList();
#endif
        }

        public void LoadHotfixAssembly() {
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"code.unity3d");
#if ILRuntime
            Log.Debug($"当前使用的是ILRuntime模式");
            this.appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            GameObject code = (GameObject)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("code.unity3d", "Code");
            byte[] assBytes = code.Get<TextAsset>("Hotfix.dll").bytes;
            byte[] mdbBytes = code.Get<TextAsset>("Hotfix.pdb").bytes;
            using (MemoryStream fs = new MemoryStream(assBytes)) // 加载热更新的程序集
                using (MemoryStream p = new MemoryStream(mdbBytes)) {
                    this.appDomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
            this.start = new ILStaticMethod(this.appDomain, "ETHotfix.Init", "Start", 0); // 通过入口方法，进入到热更新的程序域里去
#else
            Log.Debug($"当前使用的是Mono模式");
            GameObject code = (GameObject)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("code.unity3d", "Code");
            byte[] assBytes = code.Get<TextAsset>("Hotfix.dll").bytes;
            byte[] mdbBytes = code.Get<TextAsset>("Hotfix.mdb").bytes;
            this.assembly = Assembly.Load(assBytes, mdbBytes);
            Type hotfixInit = this.assembly.GetType("ETHotfix.Init");
            this.start = new MonoStaticMethod(hotfixInit, "Start");
#endif
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"code.unity3d");
        }
    }
}