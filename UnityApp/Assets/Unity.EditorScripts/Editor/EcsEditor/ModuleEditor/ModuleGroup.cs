using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

namespace ECSEditor
{
    [Serializable]
    public class FolderData
    {
        [HorizontalGroup("Split"), HideLabel]
        public string FolderPath;
        [HorizontalGroup("Split"), HideLabel]
        public UnityEngine.Object Folder;
    }

    [Serializable]
    public class ModuleData
    {
        public ModuleGroup ModuleGroup { get; set; }

        [HorizontalGroup("Horizontal", width: 180)]
        [HideLabel, ReadOnly]
        public string ModuleId;

        [HorizontalGroup("Horizontal", width: 40)]
        [HideLabel, ReadOnly]
        public string ModuleVersion;

        public string ModuleName { get { return ModuleId.Replace("com.module.", ""); } }

        //判断模块是否已安装
        private bool IsInstalled
        {
            get
            {
                if (ModuleGroup == null)
                {
                    return false;
                }
                return ModuleGroup.IsModuleInstalled(ModuleName);
            }
        }

        //判断模块是否需要更新
        private bool NeedUpdate
        {
            get
            {
                if (ModuleGroup == null)
                {
                    return false;
                }
                return ModuleGroup.IsModuleNeedUpdate(ModuleName, ModuleVersion);
            }
        }

        /// <summary>
        /// 卸载模块
        /// 删除Assets/Game.Model/thirdparty.model/com.model.**目录
        /// 删除Assets/Game.System/thirdparty.system/com.system.**目录
        /// </summary>
        [HorizontalGroup("Horizontal/Btns")]
        [Button("卸载", ButtonStyle.Box, Expanded = false)]
        [ShowIf("@IsInstalled")]
        private void Uninstall()
        {
            DeleteModule();
            Debug.Log($"模块 {ModuleId} 卸载完成");
            AssetDatabase.Refresh();
        }

        private void DeleteModule()
        {
            // 获取模块名称
            string moduleName = ModuleName;
            string targetModelRoot = Path.Combine(Application.dataPath, "App.Model/Game.Model/base-module.thirdparty.model");
            string targetSystemRoot = Path.Combine(Application.dataPath, "App.System/Game.System/base-module.thirdparty.system");
            string assetsModelRoot = Path.Combine("Assets", "App.Model/Game.Model/base-module.thirdparty.model");
            string assetsSystemRoot = Path.Combine("Assets", "App.System/Game.System/base-module.thirdparty.system");
            // 1. 删除 Assets/Game.Model/thirdparty.model/com.model.** 目录
            string modelDir = Path.Combine(targetModelRoot, $"com.model.{moduleName}");
            if (Directory.Exists(modelDir))
            {
                // Debug.Log($"删除 {modelDir}");
                //Directory.Delete(modelDir, true);
                //File.Delete(modelDir);
                //File.Delete(modelDir + ".meta");
                AssetDatabase.DeleteAsset(Path.Combine(assetsModelRoot, $"com.model.{moduleName}"));
            }
            // 2. 删除 Assets/Game.System/thirdparty.system/com.system.** 目录
            string systemDir = Path.Combine(targetSystemRoot, $"com.system.{moduleName}");
            if (Directory.Exists(systemDir))
            {
                // Debug.Log($"删除 {systemDir}");
                //Directory.Delete(systemDir, true);
                //File.Delete(systemDir);
                //File.Delete(systemDir + ".meta");
                AssetDatabase.DeleteAsset(Path.Combine(assetsSystemRoot, $"com.system.{moduleName}"));
            }

            if (ModuleGroup.InstalledModuleName2Versions.TryGetValue(moduleName, out string version))
            {
                modelDir = Path.Combine(targetModelRoot, $"com.model.{moduleName}@{version}");
                if (Directory.Exists(modelDir))
                {
                    // Debug.Log($"删除 {modelDir}");
                    //Directory.Delete(modelDir, true);
                    //File.Delete(modelDir);
                    //File.Delete(modelDir + ".meta");
                    AssetDatabase.DeleteAsset(Path.Combine(assetsModelRoot, $"com.model.{moduleName}@{version}"));
                }
                systemDir = Path.Combine(targetSystemRoot, $"com.system.{moduleName}@{version}");
                if (Directory.Exists(systemDir))
                {
                    // Debug.Log($"删除 {systemDir}");
                    //Directory.Delete(systemDir, true);
                    //File.Delete(systemDir);
                    //File.Delete(systemDir + ".meta");
                    AssetDatabase.DeleteAsset(Path.Combine(assetsSystemRoot, $"com.system.{moduleName}@{version}"));
                }
            }
        }

        [HorizontalGroup("Horizontal/Btns")]
        [Button("更新", ButtonStyle.Box, Expanded = false)]
        [ShowIf("@NeedUpdate")]
        private void Update()
        {
            Install();
        }

        /// <summary>
        /// 安装模块到unity工程中
        /// com.model.**目录放到Assets/Game.Model/thirdparty.model/目录下
        /// com.system.**目录放到Assets/Game.System/thirdparty.system/目录下
        /// </summary>
        [HorizontalGroup("Horizontal/Btns")]
        [Button("导入", ButtonStyle.Box, Expanded = false)]
        [HideIf("@IsInstalled")]
        private void Install()
        {
            // 1. 获取模块根目录
            string modulesRoot = Path.Combine(Application.dataPath, "../../Modules.Unity");
            string moduleDir = Path.Combine(modulesRoot, ModuleId);

            if (!Directory.Exists(moduleDir))
            {
                Debug.LogError($"模块目录不存在: {moduleDir}");
                return;
            }

            DeleteModule();

            string[] modelDirs = Directory.GetDirectories(moduleDir, "com.model.*", SearchOption.TopDirectoryOnly);
            string[] systemDirs = Directory.GetDirectories(moduleDir, "com.system.*", SearchOption.TopDirectoryOnly);
            CopyDirectorys(Path.Combine(Application.dataPath, "App.Model/Game.Model/base-module.thirdparty.model"), modelDirs);
            CopyDirectorys(Path.Combine(Application.dataPath, "App.System/Game.System/base-module.thirdparty.system"), systemDirs);

            // modelDirs = Directory.GetDirectories(moduleDir, "com.view-model.*", SearchOption.TopDirectoryOnly);
            // systemDirs = Directory.GetDirectories(moduleDir, "com.view-system.*", SearchOption.TopDirectoryOnly);
            // CopyDirectorys(Path.Combine(Application.dataPath, "App.Model/Game.ViewModel/base-module.thirdparty.view-model"), modelDirs);
            // CopyDirectorys(Path.Combine(Application.dataPath, "App.System/Game.ViewSystem/base-module.thirdparty.view-system"), systemDirs);

            Debug.Log($"模块 {ModuleId}@{ModuleVersion} 安装完成");
            AssetDatabase.Refresh();
        }

        private void CopyDirectorys(string targetSystemRoot, string[] systemDirs)
        {
            if (systemDirs == null || systemDirs.Length == 0)
            {
                return;
            }
            if (Directory.Exists(targetSystemRoot) == false)
            {
                Directory.CreateDirectory(targetSystemRoot);
            }
            //string targetSystemRoot = Path.Combine(Application.dataPath, "App.System/Game.System/game.thirdparty.system");
            foreach (string srcDir in systemDirs)
            {
                string dirName = Path.GetFileName(srcDir);
                string dstDir = Path.Combine(targetSystemRoot, $"{dirName}@{ModuleVersion}");
                var installedVersion = ModuleGroup.GetInstalledModuleVersion(ModuleName);
                string existDstDir = Path.Combine(targetSystemRoot, $"{dirName}@{installedVersion}");
                // 清除旧目录
                if (Directory.Exists(existDstDir))
                {
                    Directory.Delete(existDstDir, true);
                }
                // Debug.Log($"拷贝 {srcDir} 到 {dstDir}");
                CopyDirectory(srcDir, dstDir);
            }
        }

        /// <summary>
        /// 递归拷贝文件夹
        /// </summary>
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }

        public bool HasDependencies
        {
            get { return Dependencies != null && Dependencies.Count > 0; }
        }

        //[FoldoutGroup("Folder", Expanded = false)]
        [ReadOnly, LabelText("dependencies"), ShowIf("@HasDependencies")]
        //[HideLabel]
        //[ListDrawerSettings(ShowPaging = false, HideAddButton = true, HideRemoveButton = true, DefaultExpandedState = true)]
        public List<string> Dependencies;
    }

    [CreateAssetMenu(fileName = "ModuleGroup", menuName = "ModuleGroup")]
    public class ModuleGroup : SerializedScriptableObject
    {
        [HideReferenceObjectPicker, ListDrawerSettings(ShowPaging = false, HideAddButton = true, HideRemoveButton = true)]
        public List<ModuleData> UnityModules;

        public Dictionary<string, string> InstalledModuleName2Versions { get; set; } = new Dictionary<string, string>();

        private void OnEnable()
        {
            FindModules();
        }

        public bool IsModuleInstalled(string moduleName)
        {
            return InstalledModuleName2Versions.ContainsKey(moduleName);
        }

        public bool IsModuleNeedUpdate(string moduleName, string version)
        {
            if (InstalledModuleName2Versions.ContainsKey(moduleName))
            {
                if (string.IsNullOrEmpty(InstalledModuleName2Versions[moduleName]))
                {
                    return false; // 如果没有版本信息，则不需要更新
                }
                return InstalledModuleName2Versions[moduleName] != version;
            }
            return false;
        }

        public string GetInstalledModuleVersion(string moduleName)
        {
            if (InstalledModuleName2Versions.ContainsKey(moduleName))
            {
                if (string.IsNullOrEmpty(InstalledModuleName2Versions[moduleName]))
                {
                    return string.Empty; // 如果没有版本信息，则不需要更新
                }
                return InstalledModuleName2Versions[moduleName];
            }
            return string.Empty;
        }

        // 查找Modules.Unity目录下的模块（模块文件夹名称规则如com.module.**）
        [Button("刷新模块信息")]
        public void FindModules()
        {
            //Debug.Log("ModuleGroup OnEnable");
            InstalledModuleName2Versions.Clear();
            string modelDir = Path.Combine(Application.dataPath, $"App.Model/Game.Model/base-module.thirdparty.model/");
            var director = Directory.CreateDirectory(modelDir);
            director.GetDirectories("com.model.*", SearchOption.TopDirectoryOnly).ToList().ForEach(d =>
            {
                if (d.Name.Contains("@"))
                {
                    var arr = d.Name.Split("@");
                    string moduleName = arr[0].Replace("com.model.", "");
                    string version = arr[1];
                    InstalledModuleName2Versions.Add($"{moduleName}", version);
                }
                else
                {
                    string moduleName = d.Name.Replace("com.model.", "");
                    InstalledModuleName2Versions.Add($"{moduleName}", "");
                }
            });

            UnityModules.Clear();
            string modulesRoot = Path.Combine(Application.dataPath, "../../Modules.Unity");
            if (!Directory.Exists(modulesRoot))
            {
                Debug.LogError($"Modules root directory does not exist: {modulesRoot}");
                return;
            }

            string[] directories = Directory.GetDirectories(modulesRoot, "com.module.*", SearchOption.TopDirectoryOnly);
            foreach (string dir in directories)
            {
                //Debug.Log($"Found module directory: {dir}");
                string moduleName = Path.GetFileName(dir);
                //获取模块版本信息，模型信息都存于module.json
                string moduleJsonPath = Path.Combine(dir, "module.json");
                if (File.Exists(moduleJsonPath))
                {
                    try
                    {
                        string json = File.ReadAllText(moduleJsonPath);
                        //var jsonObj = JsonUtility.FromJson<ModuleJson>(json);
                        var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ModuleJson>(json);
                        var moduleData = new ModuleData
                        {
                            ModuleId = moduleName,
                            ModuleVersion = jsonObj.version,
                            Dependencies = new List<string>(),
                            ModuleGroup = this
                        };
                        foreach (var dependency in jsonObj.dependencies)
                        {
                            moduleData.Dependencies.Add($"{dependency.Key}:{dependency.Value}");
                        }
                        UnityModules.Add(moduleData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"读取模块版本失败: {moduleJsonPath}, {ex.Message}");
                    }
                }
            }
        }
    }

    // 辅助类定义（可放在文件末尾）
    [Serializable]
    public class ModuleJson
    {
        public string displayName;
        public string name;
        public string version;
        public Dictionary<string, string> dependencies;
    }
}
