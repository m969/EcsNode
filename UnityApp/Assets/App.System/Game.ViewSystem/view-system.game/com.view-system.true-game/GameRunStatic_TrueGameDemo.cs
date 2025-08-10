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
    public static class GameRunStatic_TrueGameDemo
    {
        public static void Init(Assembly systemAssembly)
        {
            ConsoleLog.Debug($"GameRunStatic_TrueGameDemo Init");

            var trueWorld = TrueWorldSystem.Create(EcsType.TrueWorld, systemAssembly);
            EcsObject.Init(trueWorld);
            EcsDomain.AddNode(trueWorld);
            EcsDomain.TrueWorld = trueWorld;

            var actor = ActorSystem.Create(trueWorld, trueWorld.NewEntityId());
            actor.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor, 1);
            EcsObject.Init(actor);
            UnityStatic.MyActor = actor;

            var actor1 = ActorSystem.Create(trueWorld, trueWorld.NewEntityId());
            actor1.AddComponent<FramePlayComponent>();
            CollisionSystem.SetLayer(actor1, 2);
            actor1.AddComponent<AIComponent>();
            EcsObject.Init(actor1);
            UnityStatic.OtherActor = actor1;
        }
    }
}