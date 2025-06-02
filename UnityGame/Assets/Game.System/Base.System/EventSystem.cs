using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface ISytemDispatch
    {

    }

    public interface IBeforeRunEvent : ISytemDispatch
    {
        void BeforeRunEvent(EcsEntity entity, IEventRun eventRun);
    }

    public interface IAfterRunEvent : ISytemDispatch
    {
        void AfterRunEvent(EcsEntity entity, IEventRun eventRun);
    }

    public class EventSystem : AComponentSystem<EcsNode, EventComponent>,
        IAwake<EcsNode, EventComponent>
    {
        public void Awake(EcsNode entity, EventComponent component)
        {
        }

        public static void Update(EcsNode entity, EventComponent component)
        {

        }

        public static void Dispatch<T>(EcsEntity entity, Action<T> action) where T : ISytemDispatch
        {
            if (entity == null || entity.IsDisposed)
            {
                return;
            }
            if (entity.EcsNode.EntityType2Systems.TryGetValue(entity.GetType(), out var systems))
            {
                foreach (var item in systems)
                {
                    if (item is T eventInstance)
                    {
                        action.Invoke(eventInstance);
                    }
                }
            }
        }

        public static async ETTask Run<T, A>(T eventRun, A a) where T : AEventRun<A>, new() where A : EcsEntity
        {
            eventRun.EcsNode = a.EcsNode;
            Dispatch<IBeforeRunEvent>(a, x => x.BeforeRunEvent(a, eventRun));
            await eventRun.Handle(a);
            Dispatch<IAfterRunEvent>(a, x => x.AfterRunEvent(a, eventRun));
        }

        public static async ETTask Run<T, A1, A2>(T eventRun, A1 a1, A2 a2) where T : AEventRun<A1, A2>, new() where A1 : EcsEntity
        {
            eventRun.EcsNode = a1.EcsNode;
            Dispatch<IBeforeRunEvent>(a1, x => x.BeforeRunEvent(a1, eventRun));
            await eventRun.Handle(a1, a2);
            Dispatch<IAfterRunEvent>(a1, x => x.AfterRunEvent(a1, eventRun));
        }

        public static async ETTask Run<T, A1, A2, A3>(T eventRun, A1 a1, A2 a2, A3 a3) where T : AEventRun<A1, A2, A3>, new() where A1 : EcsEntity
        {
            eventRun.EcsNode = a1.EcsNode;
            Dispatch<IBeforeRunEvent>(a1, x => x.BeforeRunEvent(a1, eventRun));
            await eventRun.Handle(a1, a2, a3);
            Dispatch<IAfterRunEvent>(a1, x => x.AfterRunEvent(a1, eventRun));
        }
    }
}
