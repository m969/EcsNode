using ECS;
using System.Collections;
using System.Collections.Generic;
using System;
using ECS.Unity;

namespace ECSGame
{
    public class Process_GameSystem
    {
        public static void Init(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameSystemInit Init");

            ecsNode.AddSystems(types.ToArray());

            EcsNodeSystem.Create(ecsNode);
            ecsNode.AddComponent<SoundComponent>();
            ecsNode.Init();

            ecsNode.EcsUpdate = new EcsNodeSystem();

            var game = TrueGameSystem.Create(ecsNode);
            game.AddComponent<PlayerInputComponent>();
            game.Init();

            var actor = ActorSystem.CreateActor(game);
            actor.AddComponent<EntityViewComponent>();
            actor.GetComponent<CollisionComponent>().Layer = 1;
            actor.Init();

            var actor1 = ActorSystem.CreateActor(game);
            actor1.AddComponent<AIComponent>();
            actor1.AddComponent<EntityViewComponent>();
            actor.GetComponent<CollisionComponent>().Layer = 2;
            actor1.Init();

            game.MyActor = actor;
            //_ = MoveSystem.MoveAsync(actor, Vector3.zero);
        }

        public static void Reload(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameViewSystemInit Reload");

            ecsNode.AddSystems(types.ToArray());
            ecsNode.EcsUpdate = new EcsNodeSystem();

            EventSystem.Reload(ecsNode);

            foreach (var item in ecsNode.Id2Children.Values)
            {
                if (item is Actor actor)
                {
                    MoveSystem.SetSpeed(actor, 2);
                }
            }
        }
    }
}
