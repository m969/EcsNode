using System.Collections.Generic;

namespace ECSGame.ChaseModule
{
    /// <summary>定义候选目标评分策略。</summary>
    public interface IPriorityRuleConfig
    {
        /// <summary>候选目标优先策略。</summary>
        PriorityPolicy Policy { get; }

        /// <summary>自定义策略时的权重映射。</summary>
        Dictionary<string, float> Weights { get; }

        /// <summary>是否分数越高优先级越高。</summary>
        bool HigherIsBetter { get; }
    }
}
