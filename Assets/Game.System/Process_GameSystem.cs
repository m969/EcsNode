using ECS;
using System.Collections;
using System.Collections.Generic;
using System;
using ECSUnity;
using System.Reflection;

namespace ECSGame
{
    public class Process_GameSystem
    {
        public static void Init(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Init");

            var nodeType = ecsNode.GetComponent<ConfigComponent>().NodeType;

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            EcsNodeSystem.Create(ecsNode);
            ecsNode.AddComponent<SoundComponent>();
            ecsNode.Init();

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = assembly;

            ecsNode.EcsUpdate = new EcsNodeSystem();

            //if (nodeType == EcsNodeType.TrueAuthority)
            //{
            //    var game = TrueGameSystem.Create(ecsNode);
            //    game.Init();

            //    var actor = ActorSystem.Create(game, ecsNode.NewId());
            //    //actor.AddComponent<EntityViewComponent>();
            //    actor.GetComponent<CollisionComponent>().Layer = 1;
            //    actor.Init();

            //    var actor1 = ActorSystem.Create(game, ecsNode.NewId());
            //    actor1.AddComponent<AIComponent>();
            //    //actor1.AddComponent<EntityViewComponent>();
            //    actor1.GetComponent<CollisionComponent>().Layer = 2;
            //    actor1.Init();
            //}

            if (nodeType == EcsNodeType.LocalPrePlay)
            {
                var game = TrueGameSystem.Create(ecsNode);
                game.AddComponent<PlayerInputComponent>();
                game.Init();

                var actor = ActorSystem.Create(game, ecsNode.NewId());
                actor.AddComponent<FramePlayComponent>();
                actor.AddComponent<EntityViewComponent>();
                actor.GetComponent<CollisionComponent>().Layer = 1;
                actor.Init();

                var actor1 = ActorSystem.Create(game, ecsNode.NewId());
                actor1.AddComponent<FramePlayComponent>();
                actor1.AddComponent<EntityViewComponent>();
                actor1.GetComponent<CollisionComponent>().Layer = 2;
                actor1.AddComponent<AIComponent>();
                actor1.Init();

                game.MyActor = actor;
            }
        }

        public static void Reload(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Reload");

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = assembly;

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            ecsNode.EcsUpdate = new EcsNodeSystem();

            EventSystem.Reload(ecsNode);

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
