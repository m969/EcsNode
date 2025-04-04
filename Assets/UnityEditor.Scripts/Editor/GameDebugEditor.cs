using ET;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameDebugEditor : OdinEditorWindow
{
    [MenuItem("EcsNode/GameDebugEditor")]
    public static void ShowWindow()
    {
        var window = CreateWindow<GameDebugEditor>();
        window.Show();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (AppInit.NeedReload)
            {
                AppInit.NeedReload = false;

                CompileAndReload();
            }
        }
    }

    public void CopyShareScripts(string shareFolder)
    {
        var modelFolder = Application.dataPath + $"/{shareFolder}";
        var diretories = Directory.GetDirectories(modelFolder);
        foreach (var diretoryName in diretories)
        {
            var folderName = Path.GetFileName(diretoryName);
            if (folderName.Contains("View"))
            {
                continue;
            }
            if (folderName.Contains("Unity"))
            {
                continue;
            }
            var diretory = Directory.CreateDirectory(diretoryName);
            TryAddFolder(diretory.FullName.Replace(shareFolder, "Game.CompileShare"));
            var allFolders = diretory.GetDirectories("*", SearchOption.AllDirectories);
            foreach (var folder in allFolders)
            {
                TryAddFolder(folder.FullName.Replace(shareFolder, "Game.CompileShare"));
            }
            var allCsScripts = diretory.GetFiles("*.cs", SearchOption.AllDirectories).ToList();
            foreach (var file in allCsScripts)
            {
                var fullName = file.FullName;
                var newName = fullName.Replace(shareFolder, "Game.CompileShare");
                File.Copy(fullName, newName, true);
            }
        }
    }

    public void TryAddFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    [HorizontalGroup]
    [Button("CompilePlay ▶", ButtonHeight = 35)]// ◌▲◂◀◁▷◷◯≌≋≊◌↟↝↺↻⇑⇈⇡⇧⇪⇭⇮⇫⇯⇬
    public void CompileAndPlay()
    {
        //var shareFolders = new List<string>();
        //shareFolders.Add("Assets/Game.Model/Base.Model");
        //shareFolders.Add("Assets/Game.Model/Game.Map");
        //shareFolders.Add("Assets/Game.System/Base.System");
        //shareFolders.Add("Assets/Game.System/Game.MapSystem");
        //var allAssets = AssetDatabase.FindAssets("t:Script", shareFolders.ToArray());
        //var newAssets = new List<string>();
        //foreach (var item in allAssets)
        //{
        //    var path = AssetDatabase.GUIDToAssetPath(item);
        //    var newPath = path.Replace("Assets/Game.Model", "Assets/Game.CompileShare");
        //    newPath = newPath.Replace("Assets/Game.System", "Assets/Game.CompileShare");
        //    if (!AssetDatabase.CopyAsset(path, newPath))
        //    {
        //        Debug.LogError($"Failed to copy {path}");
        //    }
        //}
        //AssetDatabase.SaveAssets();

        CopyShareScripts("Game.Model");
        CopyShareScripts("Game.System");

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        BuildAssembliesHelper.DoCompile();
        if (!Application.isPlaying)
        {
            //PlayerPrefs.SetInt("GameSystemLoad", 1);
            //PlayerPrefs.SetInt("MergeSystemLoad", 0);
            EditorApplication.isPlaying = true;
        }
    }

    [HorizontalGroup]
    [Button("CompileReload ↻", ButtonHeight = 35)]
    public void CompileAndReload()
    {
        BuildAssembliesHelper.DoCompile();
        if (Application.isPlaying)
        {
            //PlayerPrefs.SetInt("GameSystemLoad", 1);
            //PlayerPrefs.SetInt("MergeSystemLoad", 0);
            var appInit = GameObject.FindFirstObjectByType<AppInit>();
            appInit.Reload();
        }
    }

    //[Button("FastCompile & Play", ButtonHeight = 25)]
    //public void FastCompileAndPlay()
    //{
    //    //BuildAssembliesHelper.DoCompile();
    //    BuildAssembliesHelper.CompileAssemblies();
    //    if (!Application.isPlaying)
    //    {
    //        PlayerPrefs.SetInt("GameSystemLoad", 0);
    //        PlayerPrefs.SetInt("MergeSystemLoad", 1);
    //        EditorApplication.isPlaying = true;
    //    }
    //}

    //[Button("FastCompile & Reload", ButtonHeight = 25)]
    //public void FastCompileAndReload()
    //{
    //    //BuildAssembliesHelper.DoCompile();
    //    BuildAssembliesHelper.CompileAssemblies();
    //    if (Application.isPlaying)
    //    {
    //        PlayerPrefs.SetInt("GameSystemLoad", 0);
    //        PlayerPrefs.SetInt("MergeSystemLoad", 1);
    //        var sample = GameObject.FindFirstObjectByType<Sample>();
    //        sample.Reload();
    //    }
    //}
}