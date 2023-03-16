using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor {
    public static class BuildHelper {

        private const string relativeDirPrefix = "../Release";
        public static string BuildFolder = "../Release/{0}/StreamingAssets/"; // F:\LandlordsCore\Release\PC\StreamingAssets : PC 平台的路径地址
        
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

        [MenuItem("Tools/web资源服务器")] // 为什么它必须得运行起来？文件服务器不运行，客户端要如何从哪里下载热更新所用到的资源包呢？
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
        // 真正的构建步骤：
        public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe, bool isContainAB) {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            string exeName = "ET";
            switch (type) { // 根据不同的平台：来添加不同的后缀 
                case PlatformType.PC:
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
            if (!Directory.Exists(fold)) { // 如何服务器的资源包文件目录不存在，创建之
                Directory.CreateDirectory(fold);
            }
// 开始资源打包
            Log.Info("开始资源打包"); // 这四条日志很容易看见
            BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget); // 首先打的是：热更新资源包，根据用户配置选项来构建的
            GenerateVersionInfo(fold); // 生成服务器码表文件
            Log.Info("完成资源打包");
            if (isContainAB) { // 打包时，是否包含服务器里的资源包文件？
                FileHelper.CleanDirectory("Assets/StreamingAssets/"); // 把原客户端的资源包目录清空
                FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/"); // 把刚才打进服务器的最新资源包，复制一份到当前客户端
            }
            if (isBuildExe) { // 打成可运行、可执行文件：需要 BuildPlayer(...)
                // 拿到的是：游戏过程中，需要用到的场景，需要打进可执行文件。【这里不是很明白细节】
                // 返回的是：The scenes to be included in the build. If empty, the currently open scene will be built. 
                string[] levels = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
                Log.Info("开始EXE打包");
                BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
                Log.Info("完成exe打包");
            }
        }
        private static void GenerateVersionInfo(string dir) { // 生成服务器码表文件
            VersionConfig versionProto = new VersionConfig(); // 生成服务器码表文件的空壳：空字典
            GenerateVersionProto(dir, versionProto, "");      // 填充服务器码表文件
            using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create)) { // 创建码表文件：并写入数据
                byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
        private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath) {
            foreach (string file in Directory.GetFiles(dir)) { // 遍历服务器里的每个资源包文件
                string md5 = MD5Helper.FileMD5(file); // 计算当前资源包文件的 MD5 码值 
                FileInfo fi = new FileInfo(file);      // 生成当前资源包文件相关信息
                long size = fi.Length; // 文件大小
                string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}"; // 相对文件名
                versionProto.FileInfoDict.Add(filePath, new FileVersionInfo { // 将每个资源包文件信息填充入管理器
                    File = filePath,
                    MD5 = md5,
                    Size = size,
                });
            }
// 这个设计比较好玩就是：它对所有嵌套的子文件夹（子子文件夹等等，回归）里的文件同件填充到这个管理码表里，以包含相对路径的文件名相区分不同的文件夹
            foreach (string directory in Directory.GetDirectories(dir)) {
                DirectoryInfo dinfo = new DirectoryInfo(directory);
                string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
                GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel); // 回归
            }
        }
    }
}
