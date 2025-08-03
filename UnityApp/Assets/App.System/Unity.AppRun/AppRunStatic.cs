using ECS;
using ECSGame;
using FairyGUI;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    public enum GameType
    {
        ECSGame,
        TrueGameDemo,
        SimulationGameDemo,
    }

    public static class AppRunStatic
    {
        public static void Init(Assembly systemAssembly, int gameType)
        {
            ConsoleLog.Debug($"AppRunStatic Init");

            DomainEvent.InitHandlers(systemAssembly);

            if ((GameType)gameType == GameType.TrueGameDemo)
            {
                GameRunStatic_TrueGameDemo.Init(systemAssembly);
            }

            var uiStage = UISystem.Create(EcsType.UI, systemAssembly);
            EcsObject.Init(uiStage);
            EcsDomain.AddNode(uiStage);
            EcsDomain.UIStage = uiStage;

            var soundMaster = SoundSystem.Create(EcsType.Sound, systemAssembly);
            EcsObject.Init(soundMaster);
            EcsDomain.AddNode(soundMaster);
            EcsDomain.SoundMaster = soundMaster;

            if ((GameType)gameType == GameType.TrueGameDemo)
            {
                var playerInput = PlayerInputSystem.Create(EcsType.PlayerInput, systemAssembly);
                playerInput.PlayerActor = StaticObject.MyActor;
                playerInput.Game = EcsDomain.Game;
                EcsObject.Init(playerInput);
                EcsDomain.AddNode(playerInput);
                EcsDomain.PlayerInput = playerInput;
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

                var allTypes = systemAssembly.GetTypes();
                ecsNode.RegisterSystems(allTypes);
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
            EcsDomain.PlayerInput?.DriveEntityUpdate();
        }

        public static void FixedUpdate()
        {
            EcsDomain.Game?.DriveEntityFixedUpdate();
        }
    }
}