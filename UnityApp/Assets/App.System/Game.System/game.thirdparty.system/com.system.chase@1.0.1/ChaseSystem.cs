using System.Collections.Generic;
using ECS;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>提供追踪核心业务逻辑的系统。</summary>
    public partial class ChaseSystem : AComponentSystem<EcsEntity, ChaseComponent>
    {
        /// <summary>启动追踪流程。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="target">可选的初始目标。</param>
        public static void StartChase(EcsEntity entity, EcsEntity? target = null)
        {
            if (!ConditionsSystem.ShouldStart(entity))
            {
                return;
            }

            var chaseComponent = entity.GetComponent<ChaseComponent>();
            chaseComponent.IsPaused = false;

            var effectiveTarget = target;
            if (effectiveTarget != null)
            {
                SetCurrentTarget(entity, effectiveTarget);
            }
            else if (!HasActiveTarget(entity))
            {
                effectiveTarget = SelectTarget(entity);
            }
            else
            {
                effectiveTarget = GetCurrentTarget(entity);
            }

            var nextState = effectiveTarget != null ? ChaseState.Following : ChaseState.Searching;
            ChaseStateSystem.SetState(entity, nextState);

            entity.Dispatch<IOnChaseStarted>(handler => handler.OnChaseStarted(entity, effectiveTarget));
        }

        /// <summary>暂停追踪流程。</summary>
        /// <param name="entity">追踪实体。</param>
        public static void PauseChase(EcsEntity entity)
        {
            var chaseComponent = entity.GetComponent<ChaseComponent>();
            if (chaseComponent.IsPaused)
            {
                return;
            }

            chaseComponent.IsPaused = true;
            ChaseStateSystem.SetState(entity, ChaseState.Idle);
            entity.Dispatch<IOnChasePaused>(handler => handler.OnChasePaused(entity));
        }

        /// <summary>恢复追踪流程。</summary>
        /// <param name="entity">追踪实体。</param>
        public static void ResumeChase(EcsEntity entity)
        {
            var chaseComponent = entity.GetComponent<ChaseComponent>();
            if (!chaseComponent.IsPaused)
            {
                return;
            }

            chaseComponent.IsPaused = false;
            var target = GetCurrentTarget(entity);
            var nextState = target != null ? ChaseState.Following : ChaseState.Searching;
            ChaseStateSystem.SetState(entity, nextState);
            entity.Dispatch<IOnChaseResumed>(handler => handler.OnChaseResumed(entity));
        }

        /// <summary>停止追踪流程。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="reason">停止原因。</param>
        public static void StopChase(EcsEntity entity, string reason = "manual")
        {
            ClearCurrentTarget(entity);

            var chaseComponent = entity.GetComponent<ChaseComponent>();
            chaseComponent.IsPaused = false;

            ChaseStateSystem.SetState(entity, ChaseState.Idle);
            ChaseStateSystem.SetCurrentDistance(entity, 0f);
            var kinematics = new ChaseKinematics
            {
                Direction = Vector3.zero,
                Speed = 0f
            };
            ChaseStateSystem.SetKinematics(entity, in kinematics);

            entity.Dispatch<IOnChaseStopped>(handler => handler.OnChaseStopped(entity, reason));
        }

        /// <summary>逐帧更新追踪逻辑。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="deltaTime">帧间隔时间。</param>
        public static void Tick(EcsEntity entity, float deltaTime)
        {
            if (ConditionsSystem.ShouldStop(entity))
            {
                StopChase(entity, "condition");
                return;
            }

            var chaseComponent = entity.GetComponent<ChaseComponent>();
            var snapshot = CaptureRuntime(entity, chaseComponent.CurrentTargetId);
            if (snapshot.HasOwner)
            {
                if (!AreaLimitSystem.CheckInside(entity, snapshot.OwnerPosition))
                {
                    AreaLimitSystem.HandleOutOfArea(entity);
                    if (ChaseStateSystem.GetState(entity) == ChaseState.Idle)
                    {
                        return;
                    }
                }
            }

            if (chaseComponent.IsPaused)
            {
                return;
            }

            if (!HasActiveTarget(entity))
            {
                var selected = SelectTarget(entity);
                if (selected == null)
                {
                    ChaseStateSystem.SetState(entity, ChaseState.Searching);
                    return;
                }

                chaseComponent = entity.GetComponent<ChaseComponent>();
                snapshot = CaptureRuntime(entity, chaseComponent.CurrentTargetId);
            }
            else if (!snapshot.HasTarget)
            {
                snapshot = CaptureRuntime(entity, chaseComponent.CurrentTargetId);
            }

            if (!snapshot.HasTarget)
            {
                HandleLostTarget(entity, chaseComponent.CurrentTargetId);
                return;
            }

            UpdateTrackingState(entity, in snapshot);
        }

        /// <summary>更新候选列表。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="candidates">候选实体集合。</param>
        public static void UpdateTargetCandidates(EcsEntity entity, List<EcsEntity> candidates)
        {
            TargetCandidatesSystem.SetCandidates(entity, candidates);
            TargetCandidatesSystem.Score(entity);

            var chaseComponent = entity.GetComponent<ChaseComponent>();
            var config = ChaseConfigSystem.GetConfig(entity);
            if (HasActiveTarget(entity))
            {
                if (TargetCandidatesSystem.Contains(entity, chaseComponent.CurrentTargetId))
                {
                    return;
                }

                if (!config.AutoReselectOnInvalid)
                {
                    return;
                }
            }

            SelectTarget(entity);
        }

        /// <summary>选出最佳目标。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>选中的目标实体。</returns>
        public static EcsEntity? SelectTarget(EcsEntity entity)
        {
            TargetCandidatesSystem.Score(entity);
            var target = TargetCandidatesSystem.Pick(entity);
            if (target != null)
            {
                SetCurrentTarget(entity, target);
                return target;
            }

            ClearCurrentTarget(entity);
            ChaseStateSystem.SetState(entity, ChaseState.Searching);
            return null;
        }

        /// <summary>获取当前追踪状态。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>追踪状态。</returns>
        public static ChaseState GetState(EcsEntity entity)
        {
            return ChaseStateSystem.GetState(entity);
        }

        /// <summary>获取当前目标实体。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>目标实体。</returns>
        public static EcsEntity? GetCurrentTarget(EcsEntity entity)
        {
            return ResolveTarget(entity, entity.GetComponent<ChaseComponent>().CurrentTargetId);
        }

        /// <summary>当前是否存在有效目标。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>是否存在目标。</returns>
        public static bool HasActiveTarget(EcsEntity entity)
        {
            return entity.GetComponent<ChaseComponent>().CurrentTargetId > 0;
        }

        /// <summary>获取当前追踪距离。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>与目标的距离。</returns>
        public static float GetCurrentDistance(EcsEntity entity)
        {
            return ChaseStateSystem.GetCurrentDistance(entity);
        }

        /// <summary>获取当前速度向量。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>速度向量。</returns>
        public static Vector3 GetVelocity(EcsEntity entity)
        {
            return ChaseStateSystem.GetKinematics(entity).Velocity;
        }

        /// <summary>设置启动条件。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditions">启动条件列表。</param>
        public static void SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)
        {
            ConditionsSystem.SetStartConditions(entity, conditions);
        }

        /// <summary>设置停止条件。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="conditions">停止条件列表。</param>
        public static void SetStopConditions(EcsEntity entity, List<IStopConditionConfig> conditions)
        {
            ConditionsSystem.SetStopConditions(entity, conditions);
        }

        /// <summary>设置当前追踪目标。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="target">目标实体。</param>
        public static void SetCurrentTarget(EcsEntity entity, EcsEntity target)
        {
            UpdateTarget(entity, target);
            ChaseStateSystem.SetState(entity, ChaseState.Following);
        }

        private static void ClearCurrentTarget(EcsEntity entity)
        {
            UpdateTarget(entity, null);
        }

        private static void UpdateTarget(EcsEntity entity, EcsEntity? newTarget)
        {
            var chaseComponent = entity.GetComponent<ChaseComponent>();
            var oldId = chaseComponent.CurrentTargetId;
            var newId = newTarget?.Id ?? 0;
            if (oldId == newId)
            {
                return;
            }

            var oldTarget = ResolveTarget(entity, oldId);
            chaseComponent.CurrentTargetId = newId;
            chaseComponent.LastTargetChangeTime = GetCurrentTime(entity);

            entity.Dispatch<IOnChaseTargetChanged>(handler => handler.OnChaseTargetChanged(entity, oldTarget, newTarget));
        }

        private static void HandleLostTarget(EcsEntity entity, long lostTargetId)
        {
            if (lostTargetId <= 0)
            {
                return;
            }

            var lastTarget = ResolveTarget(entity, lostTargetId);
            ClearCurrentTarget(entity);
            ChaseStateSystem.SetState(entity, ChaseState.Lost);
            entity.Dispatch<IOnChaseLostTarget>(handler => handler.OnChaseLostTarget(entity, lastTarget));

            TargetCandidatesSystem.Invalidate(entity, lostTargetId);
            var config = ChaseConfigSystem.GetConfig(entity);
            if (config.AutoReselectOnInvalid && !HasActiveTarget(entity))
            {
                var nextTarget = SelectTarget(entity);
                if (nextTarget == null)
                {
                    ChaseStateSystem.SetState(entity, ChaseState.Searching);
                }
            }
            else if (!config.AutoReselectOnInvalid)
            {
                ChaseStateSystem.SetState(entity, ChaseState.Searching);
            }
        }

        private static void UpdateTrackingState(EcsEntity entity, in RuntimeSnapshot snapshot)
        {
            var previousDistance = ChaseStateSystem.GetCurrentDistance(entity);
            var distance = snapshot.Distance;
            ChaseStateSystem.SetCurrentDistance(entity, distance);

            var kinematics = new ChaseKinematics
            {
                Direction = snapshot.Direction,
                Speed = snapshot.RelativeSpeed
            };
            ChaseStateSystem.SetKinematics(entity, in kinematics);

            if (ChaseStateSystem.GetState(entity) == ChaseState.Searching)
            {
                ChaseStateSystem.SetState(entity, ChaseState.Following);
            }

            var target = GetCurrentTarget(entity);
            if (target == null)
            {
                return;
            }

            var config = ChaseConfigSystem.GetConfig(entity);

            if (previousDistance > config.EnterRadius && distance <= config.EnterRadius)
            {
                entity.Dispatch<IOnChaseEnterRadius>(handler => handler.OnChaseEnterRadius(entity, target, config.EnterRadius));
            }

            if (previousDistance > config.KeepDistance && distance <= config.KeepDistance)
            {
                entity.Dispatch<IOnChaseKeepDistanceReached>(handler => handler.OnChaseKeepDistanceReached(entity, target, config.KeepDistance));
            }

            if (distance >= config.LostDistance)
            {
                HandleLostTarget(entity, target.Id);
            }
        }

        private static float GetCurrentTime(EcsEntity entity)
        {
            var captured = false;
            var time = 0f;
            entity.Dispatch<IChaseTimeProvider>(provider =>
            {
                if (captured)
                {
                    return;
                }

                time = provider.GetTime(entity);
                captured = true;
            });

            return time;
        }

        private static EcsEntity? ResolveTarget(EcsEntity entity, long targetId)
        {
            if (targetId <= 0)
            {
                return null;
            }

            EcsEntity? resolved = null;
            entity.Dispatch<IChaseTargetResolver>(resolver =>
            {
                if (resolved == null)
                {
                    resolved = resolver.Resolve(entity, targetId);
                }
            });

            return resolved;
        }

        private static RuntimeSnapshot CaptureRuntime(EcsEntity entity, long targetId)
        {
            var snapshot = new RuntimeSnapshot();
            entity.Dispatch<IChaseRuntimeProvider>(provider =>
            {
                if (!snapshot.HasOwner)
                {
                    snapshot.OwnerPosition = provider.GetOwnerPosition(entity);
                    snapshot.HasOwner = true;
                }

                if (targetId > 0 && !snapshot.HasTarget)
                {
                    snapshot.TargetPosition = provider.GetTargetPosition(entity, targetId);
                    snapshot.RelativeSpeed = provider.GetRelativeSpeed(entity, targetId);
                    snapshot.HasTarget = true;
                }
            });

            if (!snapshot.HasTarget && targetId > 0)
            {
                snapshot.RelativeSpeed = 0f;
            }

            return snapshot;
        }
    }
}
