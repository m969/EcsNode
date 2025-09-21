using ECS;
using System.Collections.Generic;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 管理若干 UnitDispatcher 实体的列表组件。仅包含数据结构与同步根。
    /// </summary>
    public class UnitDispatcherListComponent : EcsComponent
    {
        /// <summary>以执行实体 Id 为键的执行实体引用表。</summary>
        public Dictionary<long, EcsEntity> Id2Entities { get; set; } = new Dictionary<long, EcsEntity>();

        /// <summary>以配置 Id 为键的执行实体列表。</summary>
        public Dictionary<int, List<EcsEntity>> ConfigId2Entities { get; set; } = new Dictionary<int, List<EcsEntity>>();

        /// <summary>可选的同步对象，调用方可据此进行并发保护。</summary>
        public object SyncRoot { get; set; } = new object();
    }
}
