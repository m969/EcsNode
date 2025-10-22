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
    public GameType GameType;
    public static bool NeedReload { get; set; } = false;
    public static bool NeedCompilePlay { get; set; } = false;
    public static bool NeedReloadShare { get; set; } = false;
    private float NextCheckReloadTime { get; set; }
    private Dictionary<string, string> ScriptFiles { get; set; } = new Dictionary<string, string>();
    public GameObject ReloadPanelObj;
    public Assembly SystemAssembly { get; private set; }

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

        //LoadSystemAssembly("Init");
    }

    private void LoadSystemAssembly(string method)
    {
        var assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "App.System.dll"));
        var pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "App.System.pdb"));
        var assembly = Assembly.Load(assBytes, pdbBytes);
        var methodInfo = assembly.GetType("ECSUnity.UnityAppRun").GetMethod(method);
        methodInfo.Invoke(null, new object[2] { assembly, ((int)GameType) });
        SystemAssembly = assembly;
    }

    public void Reload()
    {
        LoadSystemAssembly("Reload");
    }

    public void ReloadUI()
    {
        UnityAppRun.ReloadUI();
    }

    bool CheckScriptFiles()
    {
        var changed = false;
#if UNITY_EDITOR
        var allAssets = UnityEditor.AssetDatabase.FindAssets("t:Script", new string[] { "Assets/App.System" });
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
        if (SystemAssembly == null)
        {
#if UNITY_EDITOR
            var needCompileDll = UnityEditor.EditorPrefs.GetBool("NeedCompileDll", true);
            if (needCompileDll)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                NeedCompilePlay = true;
                //Debug.LogError("请打开GameDebugEditor窗口");
                //LoadSystemAssembly("Init");
                //UnityEditor.EditorPrefs.SetBool("NeedCompileDll", false);
            }
            else
            {
                LoadSystemAssembly("Init");
                UnityEditor.EditorPrefs.SetBool("NeedCompileDll", true);
            }
#endif
            return;
        }

        UnityAppRun.Update();

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
        if (SystemAssembly == null)
        {
            return;
        }
        UnityAppRun.FixedUpdate();
    }
}
