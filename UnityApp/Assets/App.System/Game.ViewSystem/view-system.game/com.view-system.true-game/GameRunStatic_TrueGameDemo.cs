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

            var game = TrueGameSystem.Create(EcsType.Game, systemAssembly);
            EcsObject.Init(game);
            EcsDomain.AddNode(game);
            EcsDomain.Game = game;

            var actor = ActorSystem.Create(game, game.NewEntityId());
            actor.AddComponent<FramePlayComponent>();
            actor.GetComponent<CollisionComponent>().Layer = 1;
            EcsObject.Init(actor);
            StaticObject.MyActor = actor;

            var actor1 = ActorSystem.Create(game, game.NewEntityId());
            actor1.AddComponent<FramePlayComponent>();
            actor1.GetComponent<CollisionComponent>().Layer = 2;
            actor1.AddComponent<AIComponent>();
            EcsObject.Init(actor1);
            StaticObject.OtherActor = actor1;
        }
    }
}