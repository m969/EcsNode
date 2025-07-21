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
    public static class AppRunStatic
    {
        public static void Init(Assembly systemAssembly)
        {
            ConsoleLog.Debug($"Process_GameRun Init");

            var uiStage = UISystem.Create(EcsType.UI, systemAssembly);
            EventSystem.Init(uiStage);
            EcsDomain.AddNode(uiStage);
            EcsDomain.UIStage = uiStage;

            var soundMaster = SoundSystem.Create(EcsType.Sound, systemAssembly);
            EventSystem.Init(soundMaster);
            EcsDomain.AddNode(soundMaster);
            EcsDomain.SoundMaster = soundMaster;

            var game = TrueGameSystem.Create(EcsType.Game, systemAssembly);
            EventSystem.Init(game);
            EcsDomain.AddNode(game);
            EcsDomain.Game = game;

            var actor = ActorSystem.Create(game, game.NewEntityId());
            actor.AddComponent<FramePlayComponent>();
            actor.GetComponent<CollisionComponent>().Layer = 1;
            EventSystem.Init(actor);
            StaticObject.MyActor = actor;

            var actor1 = ActorSystem.Create(game, game.NewEntityId());
            actor1.AddComponent<FramePlayComponent>();
            actor1.GetComponent<CollisionComponent>().Layer = 2;
            actor1.AddComponent<AIComponent>();
            EventSystem.Init(actor1);
            StaticObject.OtherActor = actor1;

            var playerInput = PlayerInputSystem.Create(EcsType.PlayerInput, systemAssembly);
            playerInput.PlayerActor = actor;
            playerInput.Game = game;
            EventSystem.Init(playerInput);
            EcsDomain.AddNode(playerInput);
            EcsDomain.PlayerInput = playerInput;

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

        public static void Reload(Assembly systemAssembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Reload");

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