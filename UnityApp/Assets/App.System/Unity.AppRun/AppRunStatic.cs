using cfg.data;
using ECS;
using ECSGame;
using FairyGUI;
using Login;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public static class AppRunStatic
    {
        public static void Init(Assembly systemAssembly, int gameType)
        {
            ConsoleLog.Debug($"AppRunStatic Init {Application.platform} {(GameType)gameType}");

            AppStatic.GameType = (GameType)gameType;

            DomainSystem.InitEventHandlers(systemAssembly);

            string gameConfDir = Path.Combine(Application.dataPath, "GameResources\\LubanConfigs\\GenerateDatas\\json"); // 替换为gen.bat中outputDataDir指向的目录
            var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
            ItemConfig.Tables = tables;
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
            EcsObject.Init(game);

            DomainViewSystem.AddUI(systemAssembly);

            DomainViewSystem.AddSound(systemAssembly);

            var groot = GRoot.inst;
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

            LoginBinder.BindAll();
            UIPackage.AddPackage("FGUI/Login");

            UISystem.Show<UI_HomePageWindow>();
        }

        public static void Reload(Assembly systemAssembly, int gameType)
        {
            ConsoleLog.Debug($"AppRunStatic Reload");

            foreach (var ecsNode in EcsDomain.EcsNodes.Values)
            {
                ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;
                EcsNodeSystem.RegisterSystems(ecsNode, systemAssembly);
            }

            //EventSystem.Reload(ecsNode);

            //ReloadUI(ecsNode);

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