using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ECSUnity;
using ECSGame;
//using Puerts;
//using Puerts.TSLoader;

//public class TestLoader : ILoader
//{
//    public Dictionary<string, string> TextMaps = new();

//    public bool FileExists(string specifier)
//    {
//        ConsoleLog.Debug($"FileExists {specifier}");
//        //return specifier.EndsWith(".mjs");
//        return false;
//    }

//    public string ReadFile(string specifier, out string debugpath)
//    {
//        if (specifier.EndsWith(".mjs"))
//        {
//            debugpath = specifier.Replace(".mjs", "");
//        }
//        else
//        {
//            debugpath = specifier;
//        }
//        //return "console.log('test Runtime')";
//        ConsoleLog.Debug($"ReadFile {debugpath}");
//        return Resources.Load<TextAsset>(debugpath).text;
//    }
//}

public class Define
{
    public static string BuildOutputDir = "./DllDatas";
}

//public class AssemblyLoader : MarshalByRefObject
//{
//    public Assembly Load(byte[] ass, byte[] pdb)
//    {
//        Assembly assembly = Assembly.Load(ass, pdb);
//        return assembly;
//    }

//    public Assembly LoadFile(string assemblyPath)
//    {
//        // 加载程序集
//        Assembly assembly = Assembly.LoadFrom(assemblyPath);

//        //// 使用反射调用方法
//        //Type type = assembly.GetType("MyNamespace.MyClass");
//        //object instance = Activator.CreateInstance(type);
//        //type.GetMethod("MyMethod").Invoke(instance, null);

//        return assembly;
//    }
//}

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

public class AppLoad : MonoBehaviour
{
    public static bool NeedReload { get; set; } = false;
    public static bool NeedReloadShare { get; set; } = false;
    private EcsNode EcsNode { get; set; }
    private EcsNode PrePlayEcsNode { get; set; }
    private AppDomain HotReloadDomain { get; set; }
    private float NextCheckReloadTime {  get; set; }
    private Dictionary<string, string> ScriptFiles {  get; set; } = new Dictionary<string, string>();
    public GameObject ReloadPanelObj;
    //public JsEnv Env { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        NeedReloadShare = true;

        ET.ETTask.ExceptionHandler = (e) =>
        {
            Debug.LogException(e);
        };

        ConsoleLog.LogAction = Debug.Log;
        ConsoleLog.LogErrorAction = Debug.LogError;
        ConsoleLog.LogExceptionAction = Debug.LogException;

        CheckScriptFiles();

        EcsNode = new EcsNode(1);
        EcsNode.Id = EcsNode.NewInstanceId();
        StaticObject.EcsNode = EcsNode;
        EcsNode.RegisterDrive<IAwake>();
        EcsNode.RegisterDrive<IEnable>();
        EcsNode.RegisterDrive<IDisable>();
        EcsNode.RegisterDrive<IDestroy>();
        EcsNode.RegisterDrive<IInit>();
        EcsNode.RegisterDrive<IUpdate>();
        EcsNode.AddComponent<ConfigComponent>(beforeAwake: x => x.NodeType = EcsNodeType.LocalPrePlay);

        //PrePlayEcsNode = new EcsNode();
        //StaticObject.PrePlayEcsNode = PrePlayEcsNode;
        //RegisterDrives(PrePlayEcsNode);
        //PrePlayEcsNode.AddComponent<ConfigComponent>(beforeAwake: x => x.NodeType = EcsNodeType.LocalPrePlay);

        Process_GameSystem.Init(EcsNode, typeof(Process_GameSystem).Assembly);
        //LoadSystemAssembly("Init");
        //if (ReloadPanelObj)
        //{
        //    ReloadPanelObj.transform.Find("Btn_Reload").GetComponent<Button>().onClick.AddListener(() =>
        //    {
        //        NeedReload = true;
        //    });
        //}

        //var loader = new TSLoader();
        //// UseRuntimeLoader在Runtime下会形成链式处理。在Editor下不生效。
        //// 执行顺序是Loader加入顺序的倒序

        //// 通过菜单命令可以快速把TS构建到Resources目录供DefaultLoader使用   
        //loader.UseRuntimeLoader(new DefaultLoader());
        //// Editor下打开PUERTS_TSLOADER_DISABLE_EDITOR_FEATURE可以测试runtime下的效果。
        //loader.UseRuntimeLoader(new TestLoader());

        //Env = new JsEnv(loader);
        ////env.ExecuteModule("test.mts");
        //Env.ExecuteModule("main.mts");
    }

    //private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    //{
    //    string assemblyName = new AssemblyName(args.Name).Name;
    //    string assemblyPath = Path.Combine(Define.BuildOutputDir, $"{assemblyName}.dll");
    //    Debug.Log($"OnAssemblyResolve {assemblyPath}");
    //    if (File.Exists(assemblyPath))
    //    {
    //        return Assembly.LoadFrom(assemblyPath);
    //    }

    //    return null;
    //}

    private void LoadSystemAssembly(string method)
    {
        var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
        var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
        var assembly = Assembly.Load(assBytes, pdbBytes);
        var methodInfo = assembly.GetType("ECSGame.Process_GameSystem").GetMethod(method);
        methodInfo.Invoke(null, new object[2] { EcsNode, assembly });
    }

    //[ContextMenu("Reload")]
    public void Reload()
    {
        Process_GameSystem.Reload(EcsNode, typeof(Process_GameSystem).Assembly);

        //LoadSystemAssembly("Reload");
        //if (ReloadPanelObj)
        //{
        //    ReloadPanelObj.gameObject.SetActive(false);
        //}
    }

    public void ReloadUI()
    {
        Process_GameSystem.ReloadUI(EcsNode);
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
        //Env?.Tick();

        //if (Time.realtimeSinceStartup > NextCheckReloadTime)
        //{
        //    NextCheckReloadTime = Time.realtimeSinceStartup + 1;
        //    var changed = CheckScriptFiles();
        //    if (ReloadPanelObj && changed)
        //    {
        //        ReloadPanelObj.gameObject.SetActive(true);
        //    }
        //}
    }

    void FixedUpdate()
    {
        //EcsNode.DriveUpdate();
    }
}
