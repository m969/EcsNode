using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// UnitDispatcher 列表组件的系统实现，提供幂等的增删查遍历方法。
    /// </summary>
    public class UnitDispatcherListSystem : AComponentSystem<EcsEntity, UnitDispatcherListComponent>,
        IAwake<EcsEntity, UnitDispatcherListComponent>, IInit<EcsEntity, UnitDispatcherListComponent>, IAfterInit<EcsEntity, UnitDispatcherListComponent>,
        IEnable<EcsEntity, UnitDispatcherListComponent>, IDisable<EcsEntity, UnitDispatcherListComponent>, IDestroy<EcsEntity, UnitDispatcherListComponent>
    {
        #region 生命周期 (空实现)
        public void Awake(EcsEntity entity, UnitDispatcherListComponent c) { }
        public void Init(EcsEntity entity, UnitDispatcherListComponent c) { }
        public void AfterInit(EcsEntity entity, UnitDispatcherListComponent c) { }
        public void Enable(EcsEntity entity, UnitDispatcherListComponent c) { }
        public void Disable(EcsEntity entity, UnitDispatcherListComponent c) { }
        public void Destroy(EcsEntity entity, UnitDispatcherListComponent c) { }
        #endregion

        /// <summary>
        /// 向列表中添加一个 UnitDispatcher 实体（幂等）。
        /// </summary>
        public static bool AddDispatcher(EcsEntity listOwner, EcsEntity exec)
        {
            var id = exec.Id;

            var listComponent = listOwner.GetComponent<UnitDispatcherListComponent>();
            lock (listComponent.SyncRoot)
            {
                if (listComponent.Id2Entities.ContainsKey(id)) return true;

                listComponent.Id2Entities[id] = exec;

                var cfgId = 0;
                if (exec is UnitDispatcher de) cfgId = de.ConfigId;

                if (!listComponent.ConfigId2Entities.TryGetValue(cfgId, out var list))
                {
                    list = new List<EcsEntity>();
                    listComponent.ConfigId2Entities[cfgId] = list;
                }
                list.Add(exec);
            }

            return true;
        }

        /// <summary>
        /// 从列表中移除一个 UnitDispatcher 实体（幂等）。
        /// </summary>
        public static bool RemoveDispatcher(EcsEntity listOwner, long execId)
        {
            var listComponent = listOwner.GetComponent<UnitDispatcherListComponent>();
            lock (listComponent.SyncRoot)
            {
                if (!listComponent.Id2Entities.TryGetValue(execId, out var exec)) return false;

                listComponent.Id2Entities.Remove(execId);

                var cfgId = 0;
                if (exec is UnitDispatcher de) cfgId = de.ConfigId;

                if (listComponent.ConfigId2Entities.TryGetValue(cfgId, out var list))
                {
                    list.RemoveAll(e => e == null || e.Id == execId);
                    if (list.Count == 0)
                    {
                        listComponent.ConfigId2Entities.Remove(cfgId);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 根据 Id 获取执行实体（若不存在返回 null）。
        /// </summary>
        public static EcsEntity? GetDispatcher(EcsEntity listOwner, long execId)
        {
            var listComponent = listOwner.GetComponent<UnitDispatcherListComponent>();
            lock (listComponent.SyncRoot)
            {
                if (listComponent.Id2Entities.TryGetValue(execId, out var exec)) return exec;
                return null;
            }
        }

        /// <summary>
        /// 按配置 Id 获取执行实体列表（若不存在返回空列表）。返回只读副本以保证调用方不破坏内部结构。
        /// </summary>
        public static IReadOnlyList<EcsEntity> GetDispatchersByConfig(EcsEntity listOwner, int configId)
        {
            var listComponent = listOwner.GetComponent<UnitDispatcherListComponent>();
            lock (listComponent.SyncRoot)
            {
                if (!listComponent.ConfigId2Entities.TryGetValue(configId, out var list)) return Array.Empty<EcsEntity>();
                return list.AsReadOnly();
            }
        }

        /// <summary>
        /// 对列表中的每个执行实体执行回调（安全遍历，忽略空引用）。
        /// </summary>
        public static void ForEach(EcsEntity listOwner, Action<EcsEntity> action)
        {
            if (action == null) return;

            var listComponent = listOwner.GetComponent<UnitDispatcherListComponent>();
            List<EcsEntity> snapshot;
            lock (listComponent.SyncRoot)
            {
                snapshot = new List<EcsEntity>(listComponent.Id2Entities.Values);
            }

            foreach (var e in snapshot)
            {
                if (e == null) continue;
                try { action(e); }
                catch { }
            }
        }
    }
}
