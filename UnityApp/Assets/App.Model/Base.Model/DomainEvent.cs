using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECS
{
    public class DomainEvent
    {
        public static Dictionary<Type, List<IEventRun>> EventHandlers { get; set; } = new();

        public static void InitHandlers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Contains(typeof(IEventRun)))
                {
                    var eventType = type.BaseType.GetGenericArguments()[1];
                    if (!EventHandlers.TryGetValue(eventType, out var handlers))
                    {
                        handlers = new List<IEventRun>();
                        EventHandlers[eventType] = handlers;
                    }
                    handlers.Add((IEventRun)Activator.CreateInstance(type));
                }
            }
        }

        public static async ETTask PublishAsync<T, A>(T domain, A eventData) where T : EcsNode where A : IDomainEvent
        {
            var eventType = eventData.GetType();
            if (EventHandlers.TryGetValue(eventType, out var handlers))
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
            if (EventHandlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var item in handlers)
                {
                    item.Handle(domain, eventData).Coroutine();
                }
            }
        }
    }
}