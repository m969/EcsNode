using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Define
{
    public static string BuildOutputDir = "./DllDatas";
}

public class AssemblyLoader : MarshalByRefObject
{
    public Assembly Load(byte[] ass, byte[] pdb)
    {
        Assembly assembly = Assembly.Load(ass, pdb);
        return assembly;
    }

    public Assembly LoadFile(string assemblyPath)
    {
        // 加载程序集
        Assembly assembly = Assembly.LoadFrom(assemblyPath);

        //// 使用反射调用方法
        //Type type = assembly.GetType("MyNamespace.MyClass");
        //object instance = Activator.CreateInstance(type);
        //type.GetMethod("MyMethod").Invoke(instance, null);

        return assembly;
    }
}

//public class ConsoleLogger : IConsoleLogger
//{
//    public void Log(object log)
//    {
//        Debug.Log(log);
//    }

//    public void LogError(object log)
//    {
//        Debug.LogError(log);
//    }
//}

public class Sample : MonoBehaviour
{
    public static bool NeedReload { get; set; } = false;
    private EcsNode EcsNode { get; set; }
    private EcsNode PrePlayEcsNode { get; set; }
    private AppDomain HotReloadDomain { get; set; }
    private float NextCheckReloadTime {  get; set; }
    private Dictionary<string, string> ScriptFiles {  get; set; } = new Dictionary<string, string>();
    public GameObject ReloadPanelObj;

    // Start is called before the first frame update
    void Start()
    {
        ET.ETTask.ExceptionHandler = (e) =>
        {
            Debug.LogException(e);
        };

        //ConsoleLog.Logger = new ConsoleLogger();
        ConsoleLog.LogAction = Debug.Log;
        ConsoleLog.LogErrorAction = Debug.LogError;
        ConsoleLog.LogExceptionAction = Debug.LogException;
        //ConsoleLog.LogAction = (log) =>
        //{
        //    Debug.Log(log);
        //};
        //ConsoleLog.LogErrorAction = (log) =>
        //{
        //    Debug.LogError(log);
        //};

        CheckScriptFiles();

        EcsNode = new EcsNode();
        StaticObject.EcsNode = EcsNode;
        RegisterDrives(EcsNode);
        EcsNode.AddComponent<ConfigComponent>(beforeAwake: x => x.NodeType = EcsNodeType.LocalPrePlay);

        //PrePlayEcsNode = new EcsNode();
        //StaticObject.PrePlayEcsNode = PrePlayEcsNode;
        //RegisterDrives(PrePlayEcsNode);
        //PrePlayEcsNode.AddComponent<ConfigComponent>(beforeAwake: x => x.NodeType = EcsNodeType.LocalPrePlay);

        LoadSystemAssembly("Init");

        if (ReloadPanelObj)
        {
            ReloadPanelObj.transform.Find("Btn_Reload").GetComponent<Button>().onClick.AddListener(() =>
            {
                //PlayerPrefs.SetInt("NeedReload", 1);
                NeedReload = true;
            });
        }
    }

    private EcsNode RegisterDrives(EcsNode ecsNode)
    {
        ecsNode.RegisterDrive<IAwake>();
        ecsNode.RegisterDrive<IDestroy>();
        ecsNode.RegisterDrive<IInit>();
        ecsNode.RegisterDrive<IUpdate>();
        return ecsNode;
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
        string assemblyName = new AssemblyName(args.Name).Name;
        string assemblyPath = Path.Combine(Define.BuildOutputDir, $"{assemblyName}.dll");
        Debug.Log($"OnAssemblyResolve {assemblyPath}");
        if (File.Exists(assemblyPath))
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        return null;
    }

    private void LoadSystemAssembly(string method)
    {
        //if (HotReloadDomain != null)
        //{
        //    AppDomain.Unload(HotReloadDomain);
        //}
        //var domain = AppDomain.CreateDomain("HotReload");
        //HotReloadDomain = domain;

        //HotReloadDomain.AssemblyResolve += OnAssemblyResolve;
        //var loader = (AssemblyLoader)domain.CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, typeof(AssemblyLoader).FullName);

        //if (PlayerPrefs.GetInt("GameSystemLoad", 0) == 1)
        //{
        //    //var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
        //    //var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
        //    //var assembly = loader.Load(assBytes, pdbBytes);
        //    var assembly = loader.LoadFile(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
        //    var allTypes = assembly.GetTypes();

        //    //assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.dll"));
        //    //pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.pdb"));
        //    //var assembly2 = loader.Load(assBytes, pdbBytes);
        //    var assembly2 = loader.LoadFile(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.pdb"));
        //    var allTypes2 = assembly2.GetTypes();

        //    var typeList = new List<Type>();
        //    typeList.AddRange(allTypes);
        //    typeList.AddRange(allTypes2);
        //    EcsNode.AddSystems(typeList.ToArray());

        //    var methodInfo = assembly.GetType("Process_GameSystemInit").GetMethod(method);
        //    var param = new object[1] { EcsNode };
        //    methodInfo.Invoke(null, param);

        //    methodInfo = assembly2.GetType("Process_GameViewSystemInit").GetMethod(method);
        //    param = new object[1] { EcsNode };
        //    methodInfo.Invoke(null, param);
        //}

        if (PlayerPrefs.GetInt("GameSystemLoad", 0) == 1)
        {
            var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
            var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
            var assembly = Assembly.Load(assBytes, pdbBytes);
            var methodInfo = assembly.GetType("ECSGame.Process_GameSystem").GetMethod(method);

            methodInfo.Invoke(null, new object[2] { EcsNode, assembly });
            //methodInfo.Invoke(null, new object[2] { PrePlayEcsNode, assembly });
        }

        //if (PlayerPrefs.GetInt("MergeSystemLoad", 0) == 1)
        //{
        //    var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "MergeSystem.dll"));
        //    var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "MergeSystem.pdb"));
        //    var assembly = Assembly.Load(assBytes, pdbBytes);
        //    var allTypes = assembly.GetTypes();

        //    //var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
        //    //var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
        //    //var assembly = Assembly.Load(assBytes, pdbBytes);
        //    //var allTypes = assembly.GetTypes();

        //    //assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.dll"));
        //    //pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.pdb"));
        //    //var assembly2 = Assembly.Load(assBytes, pdbBytes);
        //    //var allTypes2 = assembly2.GetTypes();

        //    var typeList = new List<Type>();
        //    typeList.AddRange(allTypes);
        //    //typeList.AddRange(allTypes2);
        //    EcsNode.AddSystems(typeList.ToArray());

        //    var methodInfo = assembly.GetType("Process_GameSystemInit").GetMethod(method);
        //    var param = new object[1] { EcsNode };
        //    methodInfo.Invoke(null, param);

        //    methodInfo = assembly.GetType("Process_GameViewSystemInit").GetMethod(method);
        //    param = new object[1] { EcsNode };
        //    methodInfo.Invoke(null, param);
        //}

        return;
    }

    //[ContextMenu("Reload")]
    public void Reload()
    {
        LoadSystemAssembly("Reload");
        if (ReloadPanelObj)
        {
            ReloadPanelObj.gameObject.SetActive(false);
        }
    }

    bool CheckScriptFiles()
    {
        var changed = false;
#if UNITY_EDITOR
        var allAssets = UnityEditor.AssetDatabase.FindAssets("t:Script", new string[] { "Assets/Game.System" });
        var newAssets = new List<string>();
        foreach (var item in allAssets)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(item);
            if (path.EndsWith(".cs"))
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var time = File.GetLastWriteTimeUtc(Path.Combine(Application.dataPath, "../" + path));
                if (!ScriptFiles.ContainsKey(path))
                {
                    changed = true;
                    ScriptFiles.Add(path, time.ToString());
                }
                else
                {
                    if (!ScriptFiles[path].Equals(time.ToString()))
                    {
                        changed = true;
                    }
                    ScriptFiles[path] = time.ToString();
                    //ConsoleLog.Debug($"{path} {time.ToString()}");
                }
            }
        }
#endif

        return changed;
    }

    // Update is called once per frame
    void Update()
    {
        //if (EcsNode == null)
        //{
        //    ConsoleLog.Debug("EcsNode == null");
        //    return;
        //}
        EcsNode?.DriveEntityUpdate();
        PrePlayEcsNode?.DriveEntityUpdate();

        if (Time.realtimeSinceStartup > NextCheckReloadTime)
        {
            NextCheckReloadTime = Time.realtimeSinceStartup + 1;

            var changed = CheckScriptFiles();

            if (ReloadPanelObj && changed)
            {
                ReloadPanelObj.gameObject.SetActive(true);
            }
        }
    }

    void FixedUpdate()
    {
        //EcsNode.DriveUpdate();
    }
}
