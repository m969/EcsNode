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
            ActorListSystem.AddActor(gameWorld, actor);
            actor.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor, 1);
            actor.Init();
            UnityStatic.MyActor = actor;

            var actor1 = ActorSystem.Create(gameWorld, gameWorld.NewEntityId());
            ActorListSystem.AddActor(gameWorld, actor1);
            actor1.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor1, 2);
            actor1.AddComponent<AIComponent>();
            actor1.Init();
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
