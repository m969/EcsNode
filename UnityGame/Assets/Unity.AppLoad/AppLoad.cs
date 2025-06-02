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

public class Define
{
    public static string BuildOutputDir = "./DllDatas";
}

public class AppLoad : MonoBehaviour
{
    public static bool NeedReload { get; set; } = false;
    public static bool NeedReloadShare { get; set; } = false;
    private EcsNode EcsNode { get; set; }
    private float NextCheckReloadTime { get; set; }
    private Dictionary<string, string> ScriptFiles { get; set; } = new Dictionary<string, string>();
    public GameObject ReloadPanelObj;

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

        EcsNode = Process_GameSystem.Init(typeof(Process_GameSystem).Assembly);
    }

    //private void LoadSystemAssembly(string method)
    //{
    //    var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.dll"));
    //    var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Game.System.pdb"));
    //    var assembly = Assembly.Load(assBytes, pdbBytes);
    //    var methodInfo = assembly.GetType("ECSGame.Process_GameSystem").GetMethod(method);
    //    methodInfo.Invoke(null, new object[2] { EcsNode, assembly });
    //}

    public void Reload()
    {
        Process_GameSystem.Reload(EcsNode, typeof(Process_GameSystem).Assembly);
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
        EcsNode?.DriveEntityUpdate();

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
        EcsNode?.DriveEntityFixedUpdate();
    }
}
