using ECS;
using System.Collections.Generic;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 派遣代理组件。
    /// 合并了原有的 DispatchRule, DispatchState, UnitDispatcher 功能。
    /// 直接挂载在 Unit 实体上。
    /// </summary>
    public class DispatchAgentComponent : EcsComponent
    {
        // --- 配置数据 (原 DispatchRuleComponent) ---
        /// <summary>配置 Id（规则入口）</summary>
        public int ConfigId { get; set; }

        /// <summary>运行时参数（供规则判断与扩展使用）</summary>
        public Dictionary<string, object> RuntimeParams { get; set; } = new Dictionary<string, object>();

        // --- 运行时状态 (原 DispatchStateComponent) ---
        /// <summary>派遣状态</summary>
        public DispatchState State { get; set; } = DispatchState.Idle;

        /// <summary>当前派遣目标实体 Id</summary>
        public long TargetEntityId { get; set; }

        /// <summary>剩余超时时间（秒）</summary>
        public float RemainingTimeout { get; set; }

        // --- 任务数据 (原 UnitDispatcher) ---
        /// <summary>本次执行累计派遣数量</summary>
        public int CurrentDispatchCount { get; set; }
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
