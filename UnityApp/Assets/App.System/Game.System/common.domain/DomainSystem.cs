using ECS;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrueSync;

namespace ECSGame
{
    public class DomainSystem
    {
        public static void InitEventHandlers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof(IEventRun)))
                {
                    var eventType = type.BaseType.GetGenericArguments()[1];
                    if (!EcsDomain.EventHandlers.TryGetValue(eventType, out var handlers))
                    {
                        handlers = new List<IEventRun>();
                        EcsDomain.EventHandlers[eventType] = handlers;
                    }
                    handlers.Add((IEventRun)Activator.CreateInstance(type));
                }
            }
        }

        public static async ETTask PublishAsync<T, A>(T domain, A eventData) where T : EcsNode where A : IDomainEvent
        {
            var eventType = eventData.GetType();
            if (EcsDomain.EventHandlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var item in handlers)
                {
                    await item.Handle(domain, eventData);
                }
            }
        }

        public static void Publish<T, A>(T domain, A eventData) where T : EcsNode where A : IDomainEvent
        {
            var eventType = eventData.GetType();
            if (EcsDomain.EventHandlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var item in handlers)
                {
                    item.Handle(domain, eventData).Coroutine();
                }
            }
        }

        public static Game AddGame(int gameType, Assembly systemAssembly)
        {
            var game = GameSystem.Create(systemAssembly);
            game.Type = gameType;
            EcsDomain.AddNode(game);
            EcsDomain.Game = game;
            return game;
        }

        public static World AddWorld(Assembly systemAssembly)
        {
            var gameWorld = WorldSystem.Create(systemAssembly);
            EcsObject.Init(gameWorld);
            EcsDomain.AddNode(gameWorld);
            EcsDomain.World = gameWorld;
            return gameWorld;
        }

        public static TrueWorld AddTrueWorld(Assembly systemAssembly)
        {
            var trueWorld = TrueWorldSystem.Create(systemAssembly);
            EcsObject.Init(trueWorld);
            EcsDomain.AddNode(trueWorld);
            EcsDomain.TrueWorld = trueWorld;
            return trueWorld;
        }

        public static Player AddPlayer(Assembly systemAssembly)
        {
            var player = PlayerSystem.Create(systemAssembly);
            EcsObject.Init(player);
            EcsDomain.AddNode(player);
            EcsDomain.Player = player;
            return player;
        }
    }
}
