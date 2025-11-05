using ET;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ECSEditor
{
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
            if (AppLoad.NeedCompilePlay)
            {
                AppLoad.NeedCompilePlay = false;
                FastCompileAndPlay();
            }

            if (Application.isPlaying)
            {
                if (AppLoad.NeedReload)
                {
                    AppLoad.NeedReload = false;
                    ReloadFGUI();
                }

                //if (AppLoad.NeedReloadShare)
                //{
                //    AppLoad.NeedReloadShare = false;
                //    CompileShare();
                //}
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

        //[HorizontalGroup("CompilePlay")]
        //[Button("CompilePlay", ButtonHeight = 35)]// ↻
        //public void CompileAndPlay()
        //{
        //    if (!Application.isPlaying)
        //    {
        //        BuildAssembliesHelper.DoCompile();
        //        EditorApplication.isPlaying = true;
        //    }
        //}

        //[HorizontalGroup("CompilePlay")]
        //[Button("CompileReload", ButtonHeight = 35)]// ↻
        //public void CompileAndReload()
        //{
        //    if (Application.isPlaying)
        //    {
        //        BuildAssembliesHelper.DoCompile();
        //        var appLoad = GameObject.FindFirstObjectByType<AppLoad>();
        //        appLoad.Reload();
        //    }
        //}

        [Button("FastCompile & Play", ButtonHeight = 25)]
        public void FastCompileAndPlay()
        {
            //BuildAssembliesHelper.CompileAssemblies();
            if (BuildAssembliesHelper.DoCompile() && !Application.isPlaying)
            {
                //PlayerPrefs.SetInt("GameSystemLoad", 0);
                //PlayerPrefs.SetInt("MergeSystemLoad", 1);
                EditorPrefs.SetBool("NeedCompileDll", false);
                EditorApplication.isPlaying = true;
            }
        }

        [Button("FastCompile & Reload", ButtonHeight = 25)]
        public void FastCompileAndReload()
        {
            //BuildAssembliesHelper.CompileAssemblies();
            if (BuildAssembliesHelper.DoCompile() && Application.isPlaying)
            {
                PlayerPrefs.SetInt("GameSystemLoad", 0);
                PlayerPrefs.SetInt("MergeSystemLoad", 1);
                var appLoad = GameObject.FindFirstObjectByType<AppLoad>();
                appLoad.Reload();
            }
        }

        [HorizontalGroup("Reload")]
        [Button("CompileShare", ButtonHeight = 25)]// ◌▲◂◀◁▷◷◯≌≋≊◌↟↝↺↻⇑⇈⇡⇧⇪⇭⇮⇫⇯⇬ Play ▶
        public void CompileShare()
        {
            //CopyShareScripts("Game.Model");
            //CopyShareScripts("Game.System");
            //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            BuildAssembliesHelper.CompileShareAssemblies();
            //if (!Application.isPlaying)
            //{
            //    EditorApplication.isPlaying = true;
            //}
        }

        [HorizontalGroup("Reload")]
        [Button("ReloadSystem", ButtonHeight = 25)]// ↻
        public void ReloadSystem()
        {
            if (Application.isPlaying)
            {
                var appLoad = GameObject.FindFirstObjectByType<AppLoad>();
                appLoad.Reload();
            }
        }

        //[HorizontalGroup("PuerTs")]
        //[Button("Generate d.ts", ButtonHeight = 35)]
        //public void GenerateDTS()
        //{
        //    Puerts.Editor.Generator.UnityMenu.GenerateDTS();
        //}

        //[HorizontalGroup("PuerTs")]
        //[Button("Release ts", ButtonHeight = 35)]
        //public void ReleaseToResources()
        //{
        //    Puerts.TSLoader.TSReleaser.ReleaseToResources();
        //}

        [HorizontalGroup("Reload")]
        [Button("ReloadUI", ButtonHeight = 25)]
        public void ReloadFGUI()
        {
            if (Application.isPlaying)
            {
                var appInit = GameObject.FindFirstObjectByType<AppLoad>();
                appInit.ReloadUI();
            }
        }
    }
}