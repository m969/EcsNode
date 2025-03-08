using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class Define
{
    public static string BuildOutputDir = "./DllDatas";
}

public class Sample : MonoBehaviour
{
    private static EcsNode GameEcsNode { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        GameEcsNode = new EcsNode();

        RegisterDrives(GameEcsNode);

        var result = LoadSystemAssembly();

        var methodInfo = result.Item1.GetType("Process_GameSystemInit").GetMethod("Init");
        var param = new object[1] { GameEcsNode };
        methodInfo.Invoke(null, param);

        methodInfo = result.Item2.GetType("Process_GameViewSystemInit").GetMethod("Init");
        param = new object[1] { GameEcsNode };
        methodInfo.Invoke(null, param);
    }

    private EcsNode RegisterDrives(EcsNode ecsNode)
    {
        ecsNode.RegisterDrive<IAwake>();
        ecsNode.RegisterDrive<IInit>();
        ecsNode.RegisterDrive<IUpdate>();
        return ecsNode;
    }

    private (Assembly, Assembly) LoadSystemAssembly()
    {
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
        GameEcsNode.AddSystems(typeList.ToArray());

        return (assembly, assembly2);
    }

    [ContextMenu("Reload")]
    public void Reload()
    {
        var result = LoadSystemAssembly();

        var methodInfo = result.Item1.GetType("Process_GameSystemInit").GetMethod("Reload");
        var param = new object[1] { GameEcsNode };
        methodInfo.Invoke(null, param);

        methodInfo = result.Item2.GetType("Process_GameViewSystemInit").GetMethod("Reload");
        param = new object[1] { GameEcsNode };
        methodInfo.Invoke(null, param);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEcsNode == null)
        {
            ECS.Debug.Log("EcsNode == null");
            return;
        }
        GameEcsNode.DriveUpdate();
    }

    void FixedUpdate()
    {
        //EcsNode.DriveUpdate();
    }
}
