using System;
using System.Collections.Generic;
using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>负责启动与停止条件管理的系统。</summary>
    public class ConditionsSystem : AComponentSystem<EcsEntity, ConditionsComponent>
    {
        /// <summary>替换当前启动条件列表。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditions">新启动条件集合。</param>
        public static void SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)
        {
            entity.GetComponent<ConditionsComponent>().StartConditions = new List<IStartConditionConfig>(conditions);
        }

        /// <summary>替换当前停止条件列表。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditions">新停止条件集合。</param>
        public static void SetStopConditions(EcsEntity entity, List<IStopConditionConfig> conditions)
        {
            entity.GetComponent<ConditionsComponent>().StopConditions = new List<IStopConditionConfig>(conditions);
        }

        /// <summary>判定是否满足启动条件。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>是否满足全部启动条件。</returns>
        public static bool ShouldStart(EcsEntity entity)
        {
            var component = entity.GetComponent<ConditionsComponent>();
            if (component.StartConditions.Count == 0)
            {
                return true;
            }

            foreach (var condition in component.StartConditions)
            {
                if (!EvaluateStartCondition(entity, condition))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>判定是否满足停止条件。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>是否满足任意停止条件。</returns>
        public static bool ShouldStop(EcsEntity entity)
        {
            var component = entity.GetComponent<ConditionsComponent>();
            if (component.StopConditions.Count == 0)
            {
                return false;
            }

            foreach (var condition in component.StopConditions)
            {
                if (EvaluateStopCondition(entity, condition))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool EvaluateStartCondition(EcsEntity entity, IStartConditionConfig condition)
        {
            switch (condition.ConditionType)
            {
                case "WithinEnterRadius":
                    return CheckWithinRadius(entity, condition.Params, ChaseConfigSystem.GetConfig(entity).EnterRadius);
                case "TargetAvailable":
                    return CheckTargetAvailable(entity, condition.Params);
                default:
                    return EvaluateStartByExtension(entity, condition);
            }
        }

        private static bool EvaluateStopCondition(EcsEntity entity, IStopConditionConfig condition)
        {
            switch (condition.ConditionType)
            {
                case "LeaveExitRadius":
                    return CheckLeaveRadius(entity, condition.Params, ChaseConfigSystem.GetConfig(entity).ExitRadius);
                case "TargetInvalid":
                    return CheckTargetInvalid(entity);
                default:
                    return EvaluateStopByExtension(entity, condition);
            }
        }

        private static bool CheckWithinRadius(EcsEntity entity, Dictionary<string, string> parameters, float defaultRadius)
        {
            var radius = ReadFloat(parameters, "radius", defaultRadius);
            return ChaseStateSystem.GetCurrentDistance(entity) <= radius;
        }

        private static bool CheckLeaveRadius(EcsEntity entity, Dictionary<string, string> parameters, float defaultRadius)
        {
            var radius = ReadFloat(parameters, "radius", defaultRadius);
            return ChaseStateSystem.GetCurrentDistance(entity) >= radius;
        }

        private static bool CheckTargetAvailable(EcsEntity entity, Dictionary<string, string> parameters)
        {
            var minCount = (int)ReadFloat(parameters, "min", 1f);
            if (minCount <= 1)
            {
                return TargetCandidatesSystem.HasCandidates(entity);
            }

            return TargetCandidatesSystem.GetCandidateCount(entity) >= minCount;
        }

        private static bool CheckTargetInvalid(EcsEntity entity)
        {
            return !ChaseSystem.HasActiveTarget(entity);
        }

        private static bool EvaluateStartByExtension(EcsEntity entity, IStartConditionConfig condition)
        {
            var handled = false;
            var result = true;
            entity.Dispatch<IChaseConditionEvaluator>(evaluator =>
            {
                handled = true;
                result &= evaluator.EvaluateStart(entity, condition.ConditionType, condition.Params);
            });

            return handled && result;
        }

        private static bool EvaluateStopByExtension(EcsEntity entity, IStopConditionConfig condition)
        {
            var handled = false;
            var result = false;
            entity.Dispatch<IChaseConditionEvaluator>(evaluator =>
            {
                handled = true;
                result |= evaluator.EvaluateStop(entity, condition.ConditionType, condition.Params);
            });

            return handled && result;
        }

        private static float ReadFloat(Dictionary<string, string> parameters, string key, float fallback)
        {
            if (parameters != null && parameters.TryGetValue(key, out var value) && float.TryParse(value, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }
    }
}
