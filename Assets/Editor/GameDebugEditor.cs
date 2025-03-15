using ET;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class GameDebugEditor : OdinEditorWindow
{
    [MenuItem("ECSNode/GameDebugEditor")]
    public static void ShowWindow()
    {
        var window = CreateWindow<GameDebugEditor>();
        window.Show();
    }

    [Button("Compile & Play", ButtonHeight = 25)]
    public void CompileAndPlay()
    {
        BuildAssembliesHelper.DoCompile();
        //BuildAssembliesHelper.CompileAssemblies();
        if (!Application.isPlaying)
        {
            PlayerPrefs.SetInt("GameSystemLoad", 1);
            PlayerPrefs.SetInt("MergeSystemLoad", 0);
            EditorApplication.isPlaying = true;
        }
    }

    [Button("Compile & Reload", ButtonHeight = 25)]
    public void CompileAndReload()
    {
        BuildAssembliesHelper.DoCompile();
        //BuildAssembliesHelper.CompileAssemblies();
        if (Application.isPlaying)
        {
            PlayerPrefs.SetInt("GameSystemLoad", 1);
            PlayerPrefs.SetInt("MergeSystemLoad", 0);
            var sample = GameObject.FindFirstObjectByType<Sample>();
            sample.Reload();
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