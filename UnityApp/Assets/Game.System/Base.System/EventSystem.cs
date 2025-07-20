using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
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

        public static void OnChange<T, T2>(T entity) where T : EcsEntity, new() where T2 : EcsComponent, new()
        {
            if (entity.Components.TryGetValue(typeof(T2), out var component))
            {
                entity.EcsNode.DriveComponentSystems(entity, component, typeof(IOnChange));
            }
            else
            {
                // 如果没有该组件，则不执行
                return;
            }
            OnChange(entity);
        }

        public static void OnChange<T>(T entity) where T : EcsEntity, new()
        {
            entity.EcsNode.DriveEntitySystems(entity, typeof(IOnChange));
        }

        public static void Update(EcsNode entity)
        {

        }

        public static async ETTask RunAsync<T, A>(T eventRun, A a) where T : AEventRun<A>, new() where A : EcsEntity
        {
            await eventRun.Handle(a);
        }

        public static async ETTask RunAsync<T, A1, A2>(T eventRun, A1 a1, A2 a2) where T : AEventRun<A1, A2>, new() where A1 : EcsEntity
        {
            await eventRun.Handle(a1, a2);
        }

        public static async ETTask RunAsync<T, A1, A2, A3>(T eventRun, A1 a1, A2 a2, A3 a3) where T : AEventRun<A1, A2, A3>, new() where A1 : EcsEntity
        {
            await eventRun.Handle(a1, a2, a3);
        }
    }
}
