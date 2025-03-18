using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditorInternal;
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
    private EcsNode EcsNode { get; set; }
    private AppDomain HotReloadDomain { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EcsNode = new EcsNode();

        //ConsoleLog.Logger = new ConsoleLogger();
        ConsoleLog.LogAction = Debug.Log;
        ConsoleLog.LogErrorAction = Debug.LogError;
        //ConsoleLog.LogAction = (log) =>
        //{
        //    Debug.Log(log);
        //};
        //ConsoleLog.LogErrorAction = (log) =>
        //{
        //    Debug.LogError(log);
        //};

        RegisterDrives(EcsNode);

        LoadSystemAssembly("Init");
    }

    private EcsNode RegisterDrives(EcsNode ecsNode)
    {
        ecsNode.RegisterDrive<IAwake>();
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
            var allTypes = assembly.GetTypes();

            var typeList = new List<Type>();
            typeList.AddRange(allTypes);
            //typeList.AddRange(allTypes2);
            //EcsNode.AddSystems(typeList.ToArray());

            var methodInfo = assembly.GetType("ECSGame.Process_GameSystemInit").GetMethod(method);
            var param = new object[2] { EcsNode, typeList };
            methodInfo.Invoke(null, param);

            methodInfo = assembly.GetType("ECSGame.Process_GameViewSystemInit").GetMethod(method);
            param = new object[2] { EcsNode, typeList };
            methodInfo.Invoke(null, param);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (EcsNode == null)
        {
            ConsoleLog.Debug("EcsNode == null");
            return;
        }
        EcsNode.DriveEntityUpdate();
    }

    void FixedUpdate()
    {
        //EcsNode.DriveUpdate();
    }
}
