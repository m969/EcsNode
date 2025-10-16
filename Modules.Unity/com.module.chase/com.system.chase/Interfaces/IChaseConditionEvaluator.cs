using System.Collections.Generic;
using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供扩展条件判定能力的接口。</summary>
    public interface IChaseConditionEvaluator : IDispatch
    {
        /// <summary>扩展启动条件判定。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditionType">条件类型。</param>
        /// <param name="parameters">条件参数。</param>
        /// <returns>是否满足条件。</returns>
        bool EvaluateStart(EcsEntity entity, string conditionType, IReadOnlyDictionary<string, string> parameters);

        /// <summary>扩展停止条件判定。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditionType">条件类型。</param>
        /// <param name="parameters">条件参数。</param>
        /// <returns>是否满足条件。</returns>
        bool EvaluateStop(EcsEntity entity, string conditionType, IReadOnlyDictionary<string, string> parameters);
    }
}
