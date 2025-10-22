using cfg.data;
using ECS;
using ECSGame;
using FairyGUI;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public static class UnityAppRun
    {
        public static void Init(Assembly systemAssembly, int gameType)
        {
            ConsoleLog.Debug($"UnityAppRun Init {Application.platform} {(GameType)gameType}");

            AppStatic.GameType = (GameType)gameType;

            DomainSystem.InitEventHandlers(systemAssembly);

            string gameConfDir = Path.Combine(Application.dataPath, "GameResources\\LubanConfigs\\GenerateDatas\\json"); // 替换为gen.bat中outputDataDir指向的目录
            var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
            ItemConfig.Tables = tables;
            GridPlaneConfig.Tables = tables;
            ConsoleLog.Debug($"Tables loaded: {ItemConfig.DataList.Count}");

            var game = DomainSystem.AddGame(gameType, systemAssembly);
            if (gameType == (int)GameType.TrueGameDemo)
            {
                game.AddComponent<GameTrueWorldComponent>();
            }
            else
            {
                game.AddComponent<GameWorldComponent>();
            }
            game.AddComponent<GamePlayerComponent>();
            game.AddComponent<GamePlayerInputComponent>();
            game.Init();

            DomainViewSystem.AddUI(systemAssembly);

            DomainViewSystem.AddSound(systemAssembly);

            var groot = GRoot.inst;
            groot.SetContentScaleFactor(1280, 720);
            //groot.height = Screen.height; // 设置高度
            //groot.width = Screen.width; // 设置宽度
            ReloadUI();
        }

        public static void ReloadUI()
        {
            ConsoleLog.Debug("ReloadUI");
            foreach (var item in GRoot.inst.GetChildren())
            {
                item.Dispose();
            }
            var uiComp = EcsDomain.UIStage;
            uiComp.Type2Windows.Clear();
            UIObjectFactory.Clear();
            UIPackage.RemoveAllPackages();

            CommonButtonUI.CommonButtonUIBinder.BindAll();
            LoginUI.LoginUIBinder.BindAll();
            GameUI.GameUIBinder.BindAll();
            UIPackage.AddPackage("FGUI/CommonButtonUI");
            UIPackage.AddPackage("FGUI/LoginUI");
            UIPackage.AddPackage("FGUI/GameUI");

            if (AppStatic.GameType == GameType.TrueGameDemo)
            {
                TrueGameViewSystem.ReloadUI();
            }
            if (AppStatic.GameType == GameType.SimulationGameDemo)
            {
                SimulationGameViewSystem.ReloadUI();
            }
        }

        public static void Reload(Assembly systemAssembly, int gameType)
        {
            ConsoleLog.Debug($"AppRunStatic Reload");

            foreach (var ecsNode in EcsDomain.EcsNodes.Values)
            {
                ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;
                EcsNodeSystem.RegisterSystems(ecsNode, systemAssembly);
            }

            ReloadUI();

            //foreach (var item in ecsNode.Id2Children.Values)
            //{
            //    if (item is Actor actor)
            //    {
            //        MoveSystem.SetSpeed(actor, 2);
            //    }
            //}
        }

        public static void Update()
        {
            EcsDomain.Game?.DriveEntityUpdate();
        }

        public static void FixedUpdate()
        {
            EcsDomain.Game?.DriveEntityFixedUpdate();
        }
    }
}