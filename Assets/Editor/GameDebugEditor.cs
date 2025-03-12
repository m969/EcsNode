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

    [Button("Compile & Play", ButtonHeight = 35)]
    public void CompileAndPlay()
    {
        BuildAssembliesHelper.DoCompile();
        //BuildAssembliesHelper.CompileAssemblies();
        if (!Application.isPlaying)
        {
            EditorApplication.isPlaying = true;
        }
    }

    [Button("Compile & Reload", ButtonHeight = 35)]
    public void CompileAndReload()
    {
        BuildAssembliesHelper.DoCompile();
        //BuildAssembliesHelper.CompileAssemblies();
        if (Application.isPlaying)
        {
            var sample = GameObject.FindFirstObjectByType<Sample>();
            sample.Reload();
        }
    }
}