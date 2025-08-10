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
    public static class GameRunStatic_SimulationDemo
    {
        public static void Init(Assembly systemAssembly)
        {
            ConsoleLog.Debug($"GameRunStatic_SimulationDemo Init");

            var gameWorld = GameWorldSystem.Create(EcsType.GameWorld, systemAssembly);
            EcsObject.Init(gameWorld);
            EcsDomain.AddNode(gameWorld);
            EcsDomain.GameWorld = gameWorld;

            var actor = ActorSystem.Create(gameWorld, gameWorld.NewEntityId());
            CollisionSystem.SetLayer(actor, 1);
            EcsObject.Init(actor);
            UnityStatic.MyActor = actor;

            var actor1 = ActorSystem.Create(gameWorld, gameWorld.NewEntityId());
            CollisionSystem.SetLayer(actor1, 2);
            actor1.AddComponent<AIComponent>();
            EcsObject.Init(actor1);
            UnityStatic.OtherActor = actor1;
        }
    }
}