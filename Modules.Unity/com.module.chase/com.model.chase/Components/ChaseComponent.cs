using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>承载追踪配置与目标状态的核心组件。</summary>
    public class ChaseComponent : EcsComponent
    {
        /// <summary>绑定的追踪配置标识。</summary>
        public string ConfigId { get; set; } = string.Empty;

        /// <summary>当前锁定目标实体 Id。</summary>
        public long CurrentTargetId { get; set; }

        /// <summary>是否处于暂停状态。</summary>
        public bool IsPaused { get; set; }

        /// <summary>最近一次目标切换的时间戳。</summary>
        public float LastTargetChangeTime { get; set; }
    }
}
