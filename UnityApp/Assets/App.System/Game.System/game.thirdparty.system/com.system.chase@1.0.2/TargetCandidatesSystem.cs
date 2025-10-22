using System;
using System.Collections.Generic;
using ECS;
using ECSGame;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>负责管理候选目标与评分逻辑的系统。</summary>
    public partial class TargetCandidatesSystem : AComponentSystem<EcsEntity, TargetCandidatesComponent>
    {
        /// <summary>设置候选目标列表。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="candidates">候选目标实体列表。</param>
        public static void SetCandidates(EcsEntity entity, List<EcsEntity> candidates)
        {
            var component = entity.GetComponent<TargetCandidatesComponent>();
            component.CandidateIds.Clear();
            component.Id2Score.Clear();
            component.SelectedId = 0;

            var config = ChaseConfigSystem.GetConfig(entity);
            component.Rule = config.PriorityRule;

            var capacity = Math.Max(0, config.CandidateCapacity);
            for (var i = 0; i < candidates.Count; i++)
            {
                if (capacity > 0 && component.CandidateIds.Count >= capacity)
                {
                    break;
                }

                component.CandidateIds.Add(candidates[i].Id);
            }
        }

        /// <summary>对候选目标重新计算评分。</summary>
        /// <param name="entity">追踪实体。</param>
        public static void Score(EcsEntity entity)
        {
            var component = entity.GetComponent<TargetCandidatesComponent>();
            component.Id2Score.Clear();

            var rule = component.Rule ?? ChaseConfigSystem.GetConfig(entity).PriorityRule;
            component.Rule = rule;

            foreach (var targetId in component.CandidateIds)
            {
                component.Id2Score[targetId] = CalculateScore(entity, targetId, rule);
            }
        }

        /// <summary>根据当前评分选出最佳目标。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>选中的目标实体。</returns>
        public static EcsEntity? Pick(EcsEntity entity)
        {
            var component = entity.GetComponent<TargetCandidatesComponent>();
            if (component.CandidateIds.Count == 0)
            {
                component.SelectedId = 0;
                return null;
            }

            if (component.Id2Score.Count == 0)
            {
                Score(entity);
            }

            var rule = component.Rule ?? ChaseConfigSystem.GetConfig(entity).PriorityRule;
            var higherIsBetter = rule.HigherIsBetter;

            long bestId = component.CandidateIds[0];
            var bestScore = GetScore(component, bestId, entity, rule);

            for (var i = 1; i < component.CandidateIds.Count; i++)
            {
                var candidateId = component.CandidateIds[i];
                var score = GetScore(component, candidateId, entity, rule);
                if (IsBetter(higherIsBetter, score, bestScore))
                {
                    bestScore = score;
                    bestId = candidateId;
                }
            }

            component.SelectedId = bestId;
            return ResolveTarget(entity, bestId);
        }

        /// <summary>标记目标失效并根据配置执行重选。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">失效目标 Id。</param>
        public static void Invalidate(EcsEntity entity, long targetId)
        {
            var component = entity.GetComponent<TargetCandidatesComponent>();
            component.CandidateIds.Remove(targetId);
            component.Id2Score.Remove(targetId);

            if (component.SelectedId != targetId)
            {
                return;
            }

            component.SelectedId = 0;
            var config = ChaseConfigSystem.GetConfig(entity);
            if (!config.AutoReselectOnInvalid || component.CandidateIds.Count == 0)
            {
                return;
            }

            Score(entity);
            var newTarget = Pick(entity);
            if (newTarget != null)
            {
                ChaseSystem.SetCurrentTarget(entity, newTarget);
            }
        }

        /// <summary>判断是否存在候选目标。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>是否存在候选目标。</returns>
        public static bool HasCandidates(EcsEntity entity)
        {
            return entity.GetComponent<TargetCandidatesComponent>().CandidateIds.Count > 0;
        }

        /// <summary>获取候选目标数量。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>候选目标数量。</returns>
        public static int GetCandidateCount(EcsEntity entity)
        {
            return entity.GetComponent<TargetCandidatesComponent>().CandidateIds.Count;
        }

        /// <summary>判断指定目标是否仍在候选列表。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标 Id。</param>
        /// <returns>是否仍存在。</returns>
        public static bool Contains(EcsEntity entity, long targetId)
        {
            return entity.GetComponent<TargetCandidatesComponent>().CandidateIds.Contains(targetId);
        }

        private static float GetScore(TargetCandidatesComponent component, long targetId, EcsEntity entity, IPriorityRuleConfig rule)
        {
            if (!component.Id2Score.TryGetValue(targetId, out var score))
            {
                score = CalculateScore(entity, targetId, rule);
                component.Id2Score[targetId] = score;
            }

            return score;
        }

        private static bool IsBetter(bool higherIsBetter, float current, float reference)
        {
            return higherIsBetter ? current > reference : current < reference;
        }

        private static float CalculateScore(EcsEntity entity, long targetId, IPriorityRuleConfig rule)
        {
            switch (rule.Policy)
            {
                case PriorityPolicy.DistanceAsc:
                    return -RequestDistance(entity, targetId);
                case PriorityPolicy.Custom:
                    return CalculateCustomScore(entity, targetId, rule);
                default:
                    return 0f;
            }
        }

        private static float CalculateCustomScore(EcsEntity entity, long targetId, IPriorityRuleConfig rule)
        {
            if (rule.Weights == null || rule.Weights.Count == 0)
            {
                return 0f;
            }

            var score = 0f;
            foreach (var pair in rule.Weights)
            {
                var metric = RequestMetric(entity, targetId, pair.Key);
                score += metric * pair.Value;
            }

            return score;
        }

        private static float RequestDistance(EcsEntity entity, long targetId)
        {
            var ownerPosition = TransformSystem.GetPosition(entity);
            var target = ResolveTarget(entity, targetId);
            if (target == null)
            {
                return float.MaxValue;
            }

            var targetPosition = TransformSystem.GetPosition(target);
            return Vector3.Distance(ownerPosition.ToVector(), targetPosition.ToVector());
        }

        private static float RequestMetric(EcsEntity entity, long targetId, string key)
        {
            var value = float.NaN;
            entity.Dispatch<IChaseMetricProvider>(provider =>
            {
                if (float.IsNaN(value))
                {
                    value = provider.GetMetric(entity, targetId, key);
                }
            });

            return float.IsNaN(value) ? 0f : value;
        }

        private static EcsEntity? ResolveTarget(EcsEntity entity, long targetId)
        {
            if (targetId <= 0)
            {
                return null;
            }

            var world = FindWorld(entity);
            if (world == null)
            {
                return null;
            }

            return ActorListSystem.GetActor(world, targetId);
        }

        private static EcsEntity? FindWorld(EcsEntity entity)
        {
            var current = entity;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current;
        }
    }
}
