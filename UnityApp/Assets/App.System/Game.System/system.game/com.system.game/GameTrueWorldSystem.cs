using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameTrueWorldSystem : AComponentSystem<Game, GameTrueWorldComponent>,
    IInit<Game, GameTrueWorldComponent>
    {
        public void Init(Game game, GameTrueWorldComponent component)
        {
            var systemAssembly = game.GetComponent<ReloadComponent>().SystemAssembly;
            var trueWorld = TrueWorldSystem.Create(systemAssembly);
            trueWorld.Init();
            component.World = trueWorld;

            var actor = ActorSystem.Create(trueWorld, trueWorld.NewEntityId());
            ActorListSystem.AddActor(trueWorld, actor);
            actor.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor, 1);
            actor.Init();
            UnityAppStatic.MyActor = actor;

            var actor1 = ActorSystem.Create(trueWorld, trueWorld.NewEntityId());
            ActorListSystem.AddActor(trueWorld, actor1);
            actor1.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor1, 2);
            //actor1.AddComponent<AIComponent>();
            actor1.Init();
            UnityAppStatic.OtherActor = actor1;

            //var aiNode = AISystem.CreateNode<MoveInputAIAction>(AIBehaviourType.Patrol, actor1);
            AISystem.StartBehaviour<AIBehaviour_Patrol>(actor1);
        }

        public static void Update(Game game)
        {
            if (game.TryGetComponent<GameTrueWorldComponent>(out var component) == false)
            {
                return;
            }
            if (component.World == null)
            {
                return;
            }
            component.World.DriveEntityUpdate();
        }

        public static void FixedUpdate(Game game)
        {
            if (game.TryGetComponent<GameTrueWorldComponent>(out var component) == false)
            {
                return;
            }
            if (component.World == null)
            {
                return;
            }
            component.World.DriveEntityFixedUpdate();
        }
    
        public static TrueWorld GetTrueWorld(Game game)
        {
            if (game.TryGetComponent<GameTrueWorldComponent>(out var component) == false)
            {
                return null;
            }
            return component.World;
        }
    }
}
