using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    //public interface IBeforeRunEvent : IDispatch
    //{
    //    void BeforeRunEvent(EcsEntity entity, IEventRun eventRun);
    //}

    //public interface IAfterRunEvent : IDispatch
    //{
    //    void AfterRunEvent(EcsEntity entity, IEventRun eventRun);
    //}

    public class EventSystem : AEntitySystem<EcsNode>,
        IAwake<EcsNode>
    {
        public void Awake(EcsNode entity)
        {
        }

        public static void Init<T>(T entity) where T : EcsEntity
        {
            foreach (var item in entity.Components.Values)
            {
                entity.EcsNode.DriveComponentSystems(entity, item, typeof(IInit));
            }
            entity.EcsNode.DriveEntitySystems(entity, typeof(IInit));
        }

        //public static void OnChange<T, T2>(T entity) where T : EcsEntity, new() where T2 : EcsComponent, new()
        //{
        //    foreach (var item in entity.Components.Values)
        //    {
        //        entity.EcsNode.DriveComponentSystems(entity, item, typeof(IOnChange));
        //    }
        //    OnChange(entity);
        //}

        //public static void OnChange<T>(T entity) where T : EcsEntity, new()
        //{
        //    entity.EcsNode.DriveEntitySystems(entity, typeof(IOnChange));
        //}

        public static void Update(EcsNode entity)
        {

        }

        public static async ETTask Run<T, A>(T eventRun, A a) where T : AEventRun<A>, new() where A : EcsEntity
        {
            eventRun.EcsNode = a.EcsNode;
            //Dispatch<IBeforeRunEvent>(a, x => x.BeforeRunEvent(a, eventRun));
            await eventRun.Handle(a);
            //Dispatch<IAfterRunEvent>(a, x => x.AfterRunEvent(a, eventRun));
        }

        public static async ETTask Run<T, A1, A2>(T eventRun, A1 a1, A2 a2) where T : AEventRun<A1, A2>, new() where A1 : EcsEntity
        {
            eventRun.EcsNode = a1.EcsNode;
            //Dispatch<IBeforeRunEvent>(a1, x => x.BeforeRunEvent(a1, eventRun));
            await eventRun.Handle(a1, a2);
            //Dispatch<IAfterRunEvent>(a1, x => x.AfterRunEvent(a1, eventRun));
        }

        public static async ETTask Run<T, A1, A2, A3>(T eventRun, A1 a1, A2 a2, A3 a3) where T : AEventRun<A1, A2, A3>, new() where A1 : EcsEntity
        {
            eventRun.EcsNode = a1.EcsNode;
            //Dispatch<IBeforeRunEvent>(a1, x => x.BeforeRunEvent(a1, eventRun));
            await eventRun.Handle(a1, a2, a3);
            //Dispatch<IAfterRunEvent>(a1, x => x.AfterRunEvent(a1, eventRun));
        }
    }
}
