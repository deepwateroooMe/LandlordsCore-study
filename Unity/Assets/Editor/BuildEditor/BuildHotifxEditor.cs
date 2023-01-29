using System;
using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor {

    [InitializeOnLoad] // 程序起来的时候,这个就会运行; 或是它能够检测到文件有什么变化,也都会运行一下.....
    public class Startup {
        private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
        private const string CodeDir = "Assets/Res/Code/";
        private const string HotfixDll = "Unity.Hotfix.dll";
        private const string HotfixPdb = "Unity.Hotfix.pdb";
        private const string HotfixMdb = "Unity.Hotfix.dll.mdb";
        static Startup() {
            
#if ILRuntime
            // Copy最新的pdb文件
            string[] dirs =  {
                "./Temp/UnityVS_bin/Debug", 
                "./Temp/UnityVS_bin/Release", 
                "./Temp/bin/Debug", 
                "./Temp/bin/Release"
            };
            DateTime dateTime = DateTime.MinValue;
            string newestDir = "";
            foreach (string dir in dirs) {
                string dllPath = Path.Combine(dir, HotfixDll);
                if (!File.Exists(dllPath)) {
                    continue;
                }
                FileInfo fi = new FileInfo(dllPath);
                DateTime lastWriteTimeUtc = fi.LastWriteTimeUtc;
                if (lastWriteTimeUtc > dateTime) {
                    newestDir = dir;
                    dateTime = lastWriteTimeUtc;
                }
            }
            
            if (newestDir != "") {
                File.Copy(Path.Combine(newestDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
                File.Copy(Path.Combine(newestDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
                Log.Info($"ilrt 复制Hotfix.dll, Hotfix.pdb到Res/Code完成"); // <<<<<<<<<< ILRuntime 复制热更新程序集完成
            }
#else
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixMdb), Path.Combine(CodeDir, "Hotfix.mdb.bytes"), true);
            Log.Info($"mono 复制Hotfix.dll, Hotfix.mdb到Res/Code完成");
#endif
            
            AssetDatabase.Refresh (); // 资源包刷新一下
        }
    }
}