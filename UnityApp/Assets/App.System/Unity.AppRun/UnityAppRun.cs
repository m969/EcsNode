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

            // DomainSystem.InitEventHandlers(systemAssembly);

            var app = UnityAppSystem.Create(systemAssembly);
            app.AddComponent<UnityConfigComponent>();// 添加配置模块
            app.AddComponent<UnitySceneComponent>();// 添加场景模块
            app.AddComponent<UnityUIComponent>();// 添加UI模块
            app.AddComponent<UnitySoundComponent>();// 添加声音模块
            app.AddComponent<UnityInputComponent>();// 添加输入模块
            // app.AddComponent<UnityNeterComponent>();// 添加网络通讯模块
            app.Init();

            var game = DomainSystem.AddGame(gameType, systemAssembly);
            if (gameType == (int)GameType.TrueGameDemo) game.AddComponent<GameTrueWorldComponent>();
            if (gameType == (int)GameType.ECSGame) game.AddComponent<GameWorldComponent>();
            if (gameType == (int)GameType.SimulationGameDemo) game.AddComponent<GameWorldComponent>();
            game.AddComponent<GamePlayerComponent>();
            game.Init();
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
            EventBus.Instance.DriveUpdate();
            // EcsDomain.Game?.DriveEntityUpdate();
        }

        public static void FixedUpdate()
        {
            EventBus.Instance.DriveFixedUpdate();
            // EcsDomain.Game?.DriveEntityFixedUpdate();
        }
    }
}