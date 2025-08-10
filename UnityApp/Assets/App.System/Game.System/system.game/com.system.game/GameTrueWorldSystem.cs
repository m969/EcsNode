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
            var gameWorld = DomainSystem.AddTrueWorld(systemAssembly);
            component.World = gameWorld;

            var actor = ActorSystem.Create(gameWorld, gameWorld.NewEntityId());
            actor.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor, 1);
            EcsObject.Init(actor);
            UnityStatic.MyActor = actor;

            var actor1 = ActorSystem.Create(gameWorld, gameWorld.NewEntityId());
            actor1.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor1, 2);
            actor1.AddComponent<AIComponent>();
            EcsObject.Init(actor1);
            UnityStatic.OtherActor = actor1;
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
    }
}
