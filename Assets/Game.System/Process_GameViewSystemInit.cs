using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ECSGame
{
    public class Process_GameViewSystemInit
    {
        public static void Init(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameViewSystemInit Init");

            ecsNode.AddComponent<EventComponent>();
            ecsNode.AddComponent<TimerComponent>();
            ecsNode.Init();

            var game = ecsNode.AddChild<TrueGame>();
            game.AddComponent<TrueGameExecuteComponent>();
            game.AddComponent<TrueGamePlayComponent>();
            game.AddComponent<TrueGameCollisionComponent>();
            game.AddComponent<PlayerInputComponent>();
            game.Init();

            var actor = ActorSystem.CreateActor(game);
            actor.Init();

            var actor1 = ActorSystem.CreateActor(game);
            actor1.AddComponent<AIComponent>();
            actor1.Init();

            game.MyActor = actor;
            //_ = MoveSystem.MoveAsync(actor, Vector3.zero);
        }

        public static void Reload(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameViewSystemInit Reload");

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
