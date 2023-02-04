using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor {
    public static class BuildHelper {

        private const string relativeDirPrefix = "../Release";
        public static string BuildFolder = "../Release/{0}/StreamingAssets/";
        
        // [MenuItem("Tools/编译Hotfix")]
        public static void BuildHotfix() {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            string unityDir = System.Environment.GetEnvironmentVariable("Unity");
            if (string.IsNullOrEmpty(unityDir)) {
                Log.Error("没有设置Unity环境变量!");
                return;
            }
            process.StartInfo.FileName = $@"{unityDir}\Editor\Data\MonoBleedingEdge\bin\mono.exe";
            process.StartInfo.Arguments = $@"{unityDir}\Editor\Data\MonoBleedingEdge\lib\mono\xbuild\14.0\bin\xbuild.exe .\Hotfix\Unity.Hotfix.csproj";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = @".\";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string info = process.StandardOutput.ReadToEnd();
            process.Close();
            Log.Info(info);
        }

 // 这里需要去弄明白：这个可运行的程序，是如何打包成这个样子的，去找源码和原理        
        [MenuItem("Tools/web资源服务器")] // <<<<<<<<<<<<<<<<<<<< 看这个原理：为什么它必须得运行起来？因为它是打包好的可启动可运行的.exe文件，这里可启了这个进程程序
        public static void OpenFileServer() {
#if !UNITY_EDITOR_OSX
            string currentDir = System.Environment.CurrentDirectory;
            string path = Path.Combine(currentDir, @"..\FileServer\");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "FileServer.exe";
            process.StartInfo.WorkingDirectory = path;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
#else
            string path = System.Environment.CurrentDirectory + "/../FileServer/";
            ("cd " + path + " && go run FileServer.go").Bash(path, true);
#endif
        }

        public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe, bool isContainAB) {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            string exeName = "ET";
            switch (type) {
            case PlatformType.PC: // 这里是window PC 平台打出来的 ET.exe
                buildTarget = BuildTarget.StandaloneWindows64;
                exeName += ".exe";
                break;
            case PlatformType.Android:
                buildTarget = BuildTarget.Android;
                exeName += ".apk";
                break;
            case PlatformType.IOS:
                buildTarget = BuildTarget.iOS;
                break;
            case PlatformType.MacOS:
                buildTarget = BuildTarget.StandaloneOSX;
                    break;
            }
            string fold = string.Format(BuildFolder, type);
            if (!Directory.Exists(fold)) {
                Directory.CreateDirectory(fold);
            }
            
            Log.Info("开始资源打包"); // 这四条日志很容易看见
            BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);

            // 去确定：这里是服务器端，还是客户端？我觉得这里更像是服务器端，F:\ettetris\Release\PC\StreamingAssets
            // 客户端的是在不同的地址，对于Unity Editor，Windows平台，其等价于：Application.dataPath+"/StreamingAssets"
            // 客户端，本项目，应该是在：　　　　　　　　　　　　　　　　　　F:\ettetris\Release\ET_Data\StreamingAssets
            GenerateVersionInfo(fold); // 方法定义在下面：就生成了（这个服务器资源存放位置吗？）服务器端的MD5码表文件？
            Log.Info("完成资源打包");
            if (isContainAB) {
                FileHelper.CleanDirectory("Assets/StreamingAssets/");
                FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/"); // 它只是简单地把打包的资源包复制到特定可读目录下
            }
            if (isBuildExe) {
                string[] levels = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes); // 这里去拿到unity buildsettings里面的需要打包的几个场景，数组
                Log.Info("开始EXE打包");
                BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions); // 这里是window PC 平台打出来的 ET.exe
                Log.Info("完成exe打包");
            }
        }
        private static void GenerateVersionInfo(string dir) {
            VersionConfig versionProto = new VersionConfig();
            GenerateVersionProto(dir, versionProto, "");
            using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create)) {
                byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
        private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath) {
            foreach (string file in Directory.GetFiles(dir)) { // 它就遍历这个资源包文件夹里的每个文件，生成MD5码表
                string md5 = MD5Helper.FileMD5(file);
                FileInfo fi = new FileInfo(file);
                long size = fi.Length;
                string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";
                versionProto.FileInfoDict.Add(filePath, new FileVersionInfo {
                    File = filePath,
                    MD5 = md5,
                    Size = size,
                });
            }
            foreach (string directory in Directory.GetDirectories(dir)) { // 这里自动解决的文件夹的㠌套问题
                DirectoryInfo dinfo = new DirectoryInfo(directory);
                string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
                GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel); // recursion调用
            }
        }
    }
}
