using System.Collections.Generic;

namespace ECSGame.ChaseModule
{
    /// <summary>描述追踪组件使用的完整配置参数。</summary>
    public interface IChaseConfig
    {
        /// <summary>配置标识。</summary>
        string ConfigId { get; }

        /// <summary>进入判定半径。</summary>
        float EnterRadius { get; }

        /// <summary>退出判定半径。</summary>
        float ExitRadius { get; }

        /// <summary>保持距离阈值。</summary>
        float KeepDistance { get; }

        /// <summary>丢失距离阈值。</summary>
        float LostDistance { get; }

        /// <summary>目标失效时是否自动重选。</summary>
        bool AutoReselectOnInvalid { get; }

        /// <summary>候选目标容量上限。</summary>
        int CandidateCapacity { get; }

        /// <summary>活动区域限制配置。</summary>
        IChaseAreaConfig AreaLimit { get; }

        /// <summary>启动条件集合。</summary>
        List<IStartConditionConfig> StartConditions { get; }

        /// <summary>停止条件集合。</summary>
        List<IStopConditionConfig> StopConditions { get; }

        /// <summary>目标优先级规则配置。</summary>
        IPriorityRuleConfig PriorityRule { get; }
    }
}
