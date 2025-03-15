using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EventSystem : AComponentSystem<EcsNode, EventComponent>,
IAwake<EcsNode, EventComponent>
    {
        public void Awake(EcsNode entity, EventComponent component)
        {
            Reload(entity, component);
        }

        public static void Reload(EcsNode entity, EventComponent component)
        {
            var CommandHandlers = new Dictionary<Type, List<ICommandHandler>>();

            var types = entity.AllTypes;
            foreach (var item in types)
            {
                if (item.BaseType == null) continue;
                if (item.BaseType.BaseType == null) continue;
                if (typeof(ICommandHandler).IsAssignableFrom(item.BaseType) == false) continue;

                var handler = Activator.CreateInstance(item) as ICommandHandler;
                var cmdType = handler.Type;
                CommandHandlers.TryGetValue(cmdType, out var handlers);
                if (handlers == null)
                {
                    handlers = new List<ICommandHandler>();
                    CommandHandlers[cmdType] = handlers;
                }
                handlers.Add(handler);
            }

            component.CommandHandlers = CommandHandlers;
        }

        public static void Update(EcsNode entity, EventComponent component)
        {
            while (component.DispatchCommands.Count > 0)
            {
                var cmd = component.DispatchCommands.Dequeue();
                if (component.CommandHandlers.TryGetValue(cmd.GetType(), out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        handler.HandleCmd(cmd);
                    }
                }
            }
        }

        public static void Dispatch<T>(EcsNode ecsNode, T cmd) where T : struct, ICommand
        {
            ecsNode.GetComponent<EventComponent>().DispatchCommands.Enqueue(cmd);
        }

        public static void Execute<T>(EcsNode ecsNode, T cmd) where T : struct, IExecuteCommand
        {
            ecsNode.GetComponent<EventComponent>().ExecuteCommands.Enqueue(cmd);
        }
    } 
}
