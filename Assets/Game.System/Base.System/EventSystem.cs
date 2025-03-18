using ECS;
using ET;
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

        private static void BeforeRun<T>(T eventRun) where T : IEventRun
        {
            //AOGame.Root.GetComponent<EventComponent>().RunningEvents.Add(eventRun);
            //AOCmd.Dispatch(new BeforeRunEventCmd() { EventRun = eventRun });
        }

        private static void AfterRun<T>(T eventRun) where T : IEventRun
        {
            //AOCmd.Dispatch(new AfterRunEventCmd() { EventRun = eventRun });
            //AOGame.Root.GetComponent<EventComponent>().RunningEvents.Remove(eventRun);
        }

        public static async ETTask Run<T>(T eventRun) where T : AEventRun
        {
            BeforeRun(eventRun);
            await eventRun.Handle();
            AfterRun(eventRun);
        }

        public static async ETTask Run<T, A>(T eventRun, A a) where T : AEventRun<A>
        {
            BeforeRun(eventRun);
            await eventRun.Handle(a);
            AfterRun(eventRun);
        }

        public static async ETTask Run<T, A1, A2>(T eventRun, A1 a1, A2 a2) where T : AEventRun<A1, A2>
        {
            BeforeRun(eventRun);
            await eventRun.Handle(a1, a2);
            AfterRun(eventRun);
        }

        public static async ETTask Run<T, A1, A2, A3>(T eventRun, A1 a1, A2 a2, A3 a3) where T : AEventRun<A1, A2, A3>
        {
            BeforeRun(eventRun);
            await eventRun.Handle(a1, a2, a3);
            AfterRun(eventRun);
        }
    } 
}
