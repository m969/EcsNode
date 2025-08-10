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

        public static Game AddGame(Assembly systemAssembly)
        {
            var game = GameSystem.Create(systemAssembly);
            EcsObject.Init(game);
            EcsDomain.AddNode(game);
            EcsDomain.Game = game;
            return game;
        }

        public static UIStage AddUI(Assembly systemAssembly)
        {
            var uiStage = UISystem.Create(systemAssembly);
            EcsObject.Init(uiStage);
            EcsDomain.AddNode(uiStage);
            EcsDomain.UIStage = uiStage;
            return uiStage;
        }

        public static SoundMaster AddSound(Assembly systemAssembly)
        {
            var soundMaster = SoundSystem.Create(systemAssembly);
            EcsObject.Init(soundMaster);
            EcsDomain.AddNode(soundMaster);
            EcsDomain.SoundMaster = soundMaster;
            return soundMaster;
        }

        public static PlayerInput AddPlayerInput(Assembly systemAssembly)
        {
            var playerInput = PlayerInputSystem.Create(systemAssembly);
            EcsDomain.AddNode(playerInput);
            EcsDomain.PlayerInput = playerInput;
            return playerInput;
        }
    }
}
