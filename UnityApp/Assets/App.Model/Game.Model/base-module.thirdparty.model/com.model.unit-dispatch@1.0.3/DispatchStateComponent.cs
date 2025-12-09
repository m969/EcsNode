using ECS;
using System.Collections.Generic;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 派遣运行时状态组件。用于跟踪当前状态、目标与剩余时间等。
    /// 纯数据组件，不包含任何业务逻辑。
    /// </summary>
    public class DispatchStateComponent : EcsComponent
    {
        /// <summary>派遣状态枚举值</summary>
        public DispatchState State { get; set; }

        /// <summary>当前派遣目标实体 Id</summary>
        public long TargetEntityId { get; set; }

        /// <summary>剩余超时时间（秒）。用于 Tick 递减；小于等于 0 表示超时或未启用。</summary>
        public float RemainingTimeout { get; set; }
    }

    /// <summary>
    /// 派遣状态枚举。
    /// </summary>
    public enum DispatchState
    {
        /// <summary>默认/闲置状态</summary>
        Idle = 0,
        /// <summary>正在派遣中</summary>
        Dispatching = 1,
        /// <summary>已取消</summary>
        Cancelled = 2,
        /// <summary>已完成</summary>
        Completed = 3,
        /// <summary>已超时</summary>
        Timeout = 4
    }
}
