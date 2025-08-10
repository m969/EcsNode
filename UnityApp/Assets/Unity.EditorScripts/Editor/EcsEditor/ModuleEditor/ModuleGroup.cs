using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string targetModelRoot = Path.Combine(Application.dataPath, "App.Model/Game.Model/thirdparty.model");
            string targetSystemRoot = Path.Combine(Application.dataPath, "App.System/Game.System/thirdparty.system");
            // 1. 删除 Assets/Game.Model/thirdparty.model/com.model.** 目录
            string modelDir = Path.Combine(targetModelRoot, $"com.model.{moduleName}");
            if (Directory.Exists(modelDir))
            {
                Debug.Log($"删除 {modelDir}");
                Directory.Delete(modelDir, true);
                File.Delete(modelDir + ".meta");
            }
            // 2. 删除 Assets/Game.System/thirdparty.system/com.system.** 目录
            string systemDir = Path.Combine(targetSystemRoot, $"com.system.{moduleName}");
            if (Directory.Exists(systemDir))
            {
                Debug.Log($"删除 {systemDir}");
                Directory.Delete(systemDir, true);
                File.Delete(systemDir + ".meta");
            }

            if (ModuleGroup.InstalledModuleName2Versions.TryGetValue(moduleName, out string version))
            {
                modelDir = Path.Combine(targetModelRoot, $"com.model.{moduleName}@{version}");
                if (Directory.Exists(modelDir))
                {
                    Debug.Log($"删除 {modelDir}");
                    Directory.Delete(modelDir, true);
                    File.Delete(modelDir + ".meta");
                }
                systemDir = Path.Combine(targetSystemRoot, $"com.system.{moduleName}@{version}");
                if (Directory.Exists(systemDir))
                {
                    Debug.Log($"删除 {systemDir}");
                    Directory.Delete(systemDir, true);
                    File.Delete(systemDir + ".meta");
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

            // 2. 查找 com.model.* 和 com.system.* 子目录
            string[] modelDirs = Directory.GetDirectories(moduleDir, "com.model.*", SearchOption.TopDirectoryOnly);
            string[] systemDirs = Directory.GetDirectories(moduleDir, "com.system.*", SearchOption.TopDirectoryOnly);

            // 3. 拷贝 com.model.* 到 Assets/Game.Model/thirdparty.model/
            string targetModelRoot = Path.Combine(Application.dataPath, "App.Model/Game.Model/thirdparty.model");
            foreach (string srcDir in modelDirs)
            {
                string dirName = Path.GetFileName(srcDir);
                string dstDir = Path.Combine(targetModelRoot, $"{dirName}@{ModuleVersion}");
                Debug.Log($"拷贝 {srcDir} 到 {dstDir}");
                // 清除旧目录
                if (Directory.Exists(dstDir))
                {
                    Directory.Delete(dstDir, true);
                }
                CopyDirectory(srcDir, dstDir);
            }

            // 4. 拷贝 com.system.* 到 Assets/Game.System/thirdparty.system/
            string targetSystemRoot = Path.Combine(Application.dataPath, "App.System/Game.System/thirdparty.system");
            foreach (string srcDir in systemDirs)
            {
                string dirName = Path.GetFileName(srcDir);
                string dstDir = Path.Combine(targetSystemRoot, $"{dirName}@{ModuleVersion}");
                Debug.Log($"拷贝 {srcDir} 到 {dstDir}");
                // 清除旧目录
                if (Directory.Exists(dstDir))
                {
                    Directory.Delete(dstDir, true);
                }
                CopyDirectory(srcDir, dstDir);
            }

            Debug.Log($"模块 {ModuleId} 安装完成");
            AssetDatabase.Refresh();
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
            //Debug.Log("ModuleGroup OnEnable");
            InstalledModuleName2Versions.Clear();
            string modelDir = Path.Combine(Application.dataPath, $"App.Model/Game.Model/thirdparty.model/");
            var dir = Directory.CreateDirectory(modelDir);
            dir.GetDirectories("com.model.*", SearchOption.TopDirectoryOnly).ToList().ForEach(d =>
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
                    InstalledModuleName2Versions.Add($"{moduleName}", "0");
                }
            });
        }

        public bool IsModuleInstalled(string moduleName)
        {
            return InstalledModuleName2Versions.ContainsKey(moduleName);
        }

        public bool IsModuleNeedUpdate(string moduleName, string version)
        {
            if (InstalledModuleName2Versions.ContainsKey(moduleName))
            {
                return InstalledModuleName2Versions[moduleName] != version;
            }
            return false;
        }

        // 查找Modules.Unity目录下的模块（模块文件夹名称规则如com.module.**）
        [Button("刷新模块信息")]
        public void FindModules()
        {
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
