using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Build.Player;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace ET
{
    public static class BuildAssembliesHelper
    {
        public class Define
        {
            public static string BuildOutputDir = "./DllDatas";
        }
        public const string CodeDir = "Assets/Bundles/Code/";
        /// <summary>
        /// Unity线程的同步上下文
        /// </summary>
        static SynchronizationContext unitySynchronizationContext { get; set; }

        /// <summary>
        /// 程序集名字数组
        /// </summary>
        public static readonly string[] DllNames = { "Unity.Hotfix", "Unity.HotfixView", "Unity.Model", "Unity.ModelView" };

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            unitySynchronizationContext = SynchronizationContext.Current;
        }

        //[MenuItem("ET/Build Tool")]
        //public static void ShowWindow()
        //{
        //    //BuildHotfix(CodeOptimization.Debug);
        //    DoCompile();
        //}

        [MenuItem("ECSNode/Compile Systems")]
        public static void CompileAssemblies()
        {
            // 获取所有 asmdef 程序集的源代码文件
            var sourceFiles = new List<string>();
            var assemblies = CompilationPipeline.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.name.StartsWith("Game.System"))
                {
                    sourceFiles.AddRange(assembly.sourceFiles);
                }
                if (assembly.name.StartsWith("Game.ViewSystem"))
                {
                    sourceFiles.AddRange(assembly.sourceFiles);
                }
            }

            // 使用 Roslyn 编译
            var syntaxTrees = sourceFiles.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file)));
            var allAss = AppDomain.CurrentDomain.GetAssemblies();
            var reloadDll = "MergeSystem";
            var references = allAss
                .Where(a => !a.IsDynamic)
                .Where(a => a.GetName().Name != reloadDll)
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList();

            var compilation = CSharpCompilation.Create(
                reloadDll,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // 输出 DLL
            string outputPath = Path.Combine(Define.BuildOutputDir, reloadDll + ".dll");
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (result.Success)
                {
                    File.WriteAllBytes(outputPath, ms.ToArray());
                    Debug.Log("DLL 编译成功: " + outputPath);
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Debug.LogError(diagnostic.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 执行编译代码流程
        /// </summary>
        [MenuItem("ECSNode/DoCompile")]
        public static void DoCompile()
        {
            // 强制刷新一下，防止关闭auto refresh，编译出老代码
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            //UnityEditor.Compilation.CompilationPipeline.

            bool isCompileOk = CompileDlls();
            if (!isCompileOk)
            {
                return;
            }

            Debug.Log("DoCompile finished");
        }

        /// <summary>
        /// 编译成dll
        /// </summary>
        static bool CompileDlls()
        {
            // 运行时编译需要先设置为UnitySynchronizationContext, 编译完再还原为CurrentContext
            SynchronizationContext lastSynchronizationContext = Application.isPlaying ? SynchronizationContext.Current : null;
            SynchronizationContext.SetSynchronizationContext(unitySynchronizationContext);

            bool isCompileOk = false;

            try
            {
                Directory.CreateDirectory(Define.BuildOutputDir);
                //BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTarget target = BuildTarget.Stadia;
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
                ScriptCompilationSettings scriptCompilationSettings = new()
                {
                    group = group,
                    target = target,
                    extraScriptingDefines = new[] { "UNITY_COMPILE" },
                    options = EditorUserBuildSettings.development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None
                };
                ScriptCompilationResult result = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, Define.BuildOutputDir);
                isCompileOk = result.assemblies.Count > 0;
                EditorUtility.ClearProgressBar();
            }
            finally
            {
                if (lastSynchronizationContext != null)
                {
                    SynchronizationContext.SetSynchronizationContext(lastSynchronizationContext);
                }
            }

            return isCompileOk;
        }

        public static void BuildHotfix(CodeOptimization codeOptimization)
        {
            var Hotfix = "Game.System";
            string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, $"{Hotfix}_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }

            int random = new System.Random().Next(100000000, 999999999);
            string logicFile = $"{Hotfix}_{random}";

            List<string> codes;
            codes = new List<string>()
                {
                    "Assets/Game.System/",
                };

            BuildAssembliesHelper.BuildMuteAssembly(Hotfix, codes, new[] { Path.Combine(Define.BuildOutputDir, "Game.Model.dll") }, codeOptimization);

            //File.Copy(Path.Combine(Define.BuildOutputDir, $"{Hotfix}.dll"), Path.Combine(CodeDir, $"{Hotfix}.dll.bytes"), true);
            //File.Copy(Path.Combine(Define.BuildOutputDir, $"{Hotfix}.pdb"), Path.Combine(CodeDir, $"{Hotfix}.pdb.bytes"), true);
            //File.Copy(Path.Combine(Define.BuildOutputDir, $"{Hotfix}.dll"), Path.Combine(Define.BuildOutputDir, $"{logicFile}.dll"), true);
            //File.Copy(Path.Combine(Define.BuildOutputDir, $"{Hotfix}.pdb"), Path.Combine(Define.BuildOutputDir, $"{logicFile}.pdb"), true);
            Debug.Log($"copy {Hotfix}.dll to Bundles/Code success!");
        }

        private static void BuildMuteAssembly(string assemblyName, List<string> CodeDirectorys, string[] additionalReferences, CodeOptimization codeOptimization)
        {
            if (!Directory.Exists(Define.BuildOutputDir))
            {
                Directory.CreateDirectory(Define.BuildOutputDir);
            }

            List<string> scripts = new List<string>();
            for (int i = 0; i < CodeDirectorys.Count; i++)
            {
                DirectoryInfo dti = new DirectoryInfo(CodeDirectorys[i]);
                FileInfo[] fileInfos = dti.GetFiles("*.cs", System.IO.SearchOption.AllDirectories);
                for (int j = 0; j < fileInfos.Length; j++)
                {
                    scripts.Add(fileInfos[j].FullName);
                }
            }

            string dllPath = Path.Combine(Define.BuildOutputDir, $"{assemblyName}.dll");
            string pdbPath = Path.Combine(Define.BuildOutputDir, $"{assemblyName}.pdb");
            File.Delete(dllPath);
            File.Delete(pdbPath);

            Directory.CreateDirectory(Define.BuildOutputDir);

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());

            {
                var list = new List<string>
                {
                    "EcsNode.dll",
                    "Game.Model.dll",
                    "Game.System.dll",
                };
                list.Add($"Library/ScriptAssemblies/{assemblyName}.dll");
                assemblyBuilder.excludeReferences = list.ToArray();
            }

            //启用UnSafe
            assemblyBuilder.compilerOptions.AllowUnsafeCode = true;

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            assemblyBuilder.compilerOptions.CodeOptimization = codeOptimization;
            assemblyBuilder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup);
            // assemblyBuilder.compilerOptions.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_4_6;

            //assemblyBuilder.additionalReferences = additionalReferences;

            assemblyBuilder.flags = AssemblyBuilderFlags.None;
            //AssemblyBuilderFlags.None                 正常发布
            //AssemblyBuilderFlags.DevelopmentBuild     开发模式打包
            //AssemblyBuilderFlags.EditorAssembly       编辑器状态
            assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;

            assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;

            assemblyBuilder.buildTargetGroup = buildTargetGroup;

            assemblyBuilder.buildStarted += assemblyPath => Debug.LogFormat("build start：" + assemblyPath);

            assemblyBuilder.buildFinished += (assemblyPath, compilerMessages) =>
            {
                int errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
                int warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

                Debug.LogFormat("Warnings: {0} - Errors: {1}", warningCount, errorCount);

                if (warningCount > 0)
                {
                    Debug.LogFormat("有{0}个Warning!!!", warningCount);
                }

                if (errorCount > 0)
                {
                    for (int i = 0; i < compilerMessages.Length; i++)
                    {
                        if (compilerMessages[i].type == CompilerMessageType.Error)
                        {
                            string filename = Path.GetFullPath(compilerMessages[i].file);
                            Debug.LogError($"{compilerMessages[i].message} (at <a href=\"file:///{filename}/\" line=\"{compilerMessages[i].line}\">{Path.GetFileName(filename)}</a>)");
                        }
                    }
                }
            };

            //开始构建
            if (!assemblyBuilder.Build())
            {
                Debug.LogErrorFormat("build fail：" + assemblyBuilder.assemblyPath);
                return;
            }

            //while (EditorApplication.isCompiling)
            //{
            //    // 主线程sleep并不影响编译线程
            //    Thread.Sleep(1);
            //}
        }
    }
}