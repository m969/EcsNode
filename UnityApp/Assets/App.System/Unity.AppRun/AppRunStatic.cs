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

            DomainSystem.AddGame(systemAssembly);

            if ((GameType)gameType == GameType.TrueGameDemo)
            {
                GameRunStatic_TrueGameDemo.Init(systemAssembly);
            }

            DomainSystem.AddUI(systemAssembly);

            DomainSystem.AddSound(systemAssembly);

            if ((GameType)gameType == GameType.TrueGameDemo)
            {
                var playerInput = DomainSystem.AddPlayerInput(systemAssembly);
                playerInput.AddComponent<TrueGameInputComponent>();
                EcsObject.Init(playerInput);
            }
            if ((GameType)gameType == GameType.SimulationGameDemo)
            {
                var playerInput = DomainSystem.AddPlayerInput(systemAssembly);
                playerInput.AddComponent<SimulationGameInputComponent>();
                EcsObject.Init(playerInput);
            }

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
            EcsDomain.GameWorld?.DriveEntityUpdate();
            EcsDomain.TrueWorld?.DriveEntityUpdate();
            EcsDomain.PlayerInput?.DriveEntityUpdate();
        }

        public static void FixedUpdate()
        {
            EcsDomain.Game?.DriveEntityFixedUpdate();
            EcsDomain.GameWorld?.DriveEntityFixedUpdate();
            EcsDomain.TrueWorld?.DriveEntityFixedUpdate();
        }
    }
}