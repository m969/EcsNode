using ECS;
using ECSUnity;
using FairyGUI;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class Process_GameSystem
    {
        public static EcsNode Init(Assembly systemAssembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Init");

            var ecsNode = EcsNodeSystem.Create(1, systemAssembly);
            StaticObject.EcsNode = ecsNode;

            ecsNode.AddComponent<SoundComponent>();
            ecsNode.AddComponent<UIComponent>();
            ecsNode.Init();

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;

            var game = TrueGameSystem.Create(ecsNode);
            game.AddComponent<PlayerInputComponent>();
            game.Init();

            StaticObject.TrueGame = game;

            var actor = ActorSystem.Create(game, ecsNode.NewEntityId());
            actor.AddComponent<FramePlayComponent>();
            actor.GetComponent<CollisionComponent>().Layer = 1;
            actor.Init();
            game.MyActor = actor;

            var actor1 = ActorSystem.Create(game, ecsNode.NewEntityId());
            actor1.AddComponent<FramePlayComponent>();
            actor1.GetComponent<CollisionComponent>().Layer = 2;
            actor1.AddComponent<AIComponent>();
            actor1.Init();
            game.OtherActor = actor1;

            var groot = GRoot.inst;
            ReloadUI(ecsNode);

            return ecsNode;
        }

        public static void ReloadUI(EcsNode ecsNode)
        {
            ConsoleLog.Debug("ReloadUI");
            foreach (var item in GRoot.inst.GetChildren())
            {
                item.Dispose();
            }
            var uiComp = ecsNode.GetComponent<UIComponent>();
            uiComp.Type2Windows.Clear();
            UIObjectFactory.Clear();
            UIPackage.RemoveAllPackages();

            LoginBinder.BindAll();
            UIPackage.AddPackage("FGUI/Login");

            UISystem.Show<UI_HomePageWindow>(beforeAwake: x =>
            {
                x.TrueGame = StaticObject.TrueGame;
            });
        }

        public static void Reload(EcsNode ecsNode, Assembly systemAssembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Reload");

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;

            var allTypes = systemAssembly.GetTypes();
            ecsNode.RegisterSystems(allTypes);

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
    }
}
