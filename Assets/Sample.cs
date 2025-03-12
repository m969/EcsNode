using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

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

    public Assembly LoadAndRun(string assemblyPath)
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
    private EcsNode EcsNode { get; set; }
    //private AppDomain HotReloadDomain { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EcsNode = new EcsNode();

        //ConsoleLog.Logger = new ConsoleLogger();
        ConsoleLog.LogAction = (log) =>
        {
            Debug.Log(log);
        };
        ConsoleLog.LogErrorAction = (log) =>
        {
            Debug.LogError(log);
        };

        RegisterDrives(EcsNode);

        var result = LoadSystemAssembly("Init");

        //var methodInfo = result.Item1.GetType("Process_GameSystemInit").GetMethod("Init");
        //var param = new object[1] { EcsNode };
        //methodInfo.Invoke(null, param);

        //methodInfo = result.Item1.GetType("Process_GameViewSystemInit").GetMethod("Init");
        //param = new object[1] { EcsNode };
        //methodInfo.Invoke(null, param);
    }

    private EcsNode RegisterDrives(EcsNode ecsNode)
    {
        ecsNode.RegisterDrive<IAwake>();
        ecsNode.RegisterDrive<IInit>();
        ecsNode.RegisterDrive<IUpdate>();
        return ecsNode;
    }

    private (Assembly, Assembly) LoadSystemAssembly(string method)
    {
        //        if (HotReloadDomain != null)
        //        {
        //            // 卸载 AppDomain
        //            AppDomain.Unload(HotReloadDomain);
        //        }
        //#if UNITY_EDITOR
        //        AssetDatabase.Refresh();
        //#endif
        //        var domain = AppDomain.CreateDomain("HotReload");
        //        HotReloadDomain = domain;

        // 加载程序集
        //var loader = (AssemblyLoader)domain.CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, typeof(AssemblyLoader).FullName);

        //var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "MergeSystem.dll"));
        //var assembly = Assembly.Load(assBytes);
        //var allTypes = assembly.GetTypes();

        var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
        var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
        var assembly = Assembly.Load(assBytes, pdbBytes);
        var allTypes = assembly.GetTypes();

        assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.dll"));
        pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.ViewSystem.pdb"));
        var assembly2 = Assembly.Load(assBytes, pdbBytes);
        var allTypes2 = assembly2.GetTypes();

        var typeList = new List<Type>();
        typeList.AddRange(allTypes);
        typeList.AddRange(allTypes2);
        EcsNode.AddSystems(typeList.ToArray());

        var methodInfo = assembly.GetType("Process_GameSystemInit").GetMethod(method);
        var param = new object[1] { EcsNode };
        methodInfo.Invoke(null, param);

        methodInfo = assembly2.GetType("Process_GameViewSystemInit").GetMethod(method);
        param = new object[1] { EcsNode };
        methodInfo.Invoke(null, param);

        return (assembly, assembly2);
    }

    //[ContextMenu("Reload")]
    public void Reload()
    {
        var result = LoadSystemAssembly("Reload");

        //var methodInfo = result.Item1.GetType("Process_GameSystemInit").GetMethod("Reload");
        //var param = new object[1] { EcsNode };
        //methodInfo.Invoke(null, param);

        //methodInfo = result.Item1.GetType("Process_GameViewSystemInit").GetMethod("Reload");
        //param = new object[1] { EcsNode };
        //methodInfo.Invoke(null, param);
    }

    // Update is called once per frame
    void Update()
    {
        if (EcsNode == null)
        {
            ConsoleLog.Debug("EcsNode == null");
            return;
        }
        EcsNode.DriveUpdate();
    }

    void FixedUpdate()
    {
        //EcsNode.DriveUpdate();
    }
}
