using ECS;
using ECSGame;
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
            var chaseComponent = entity.GetComponent<ChaseComponent>();
            chaseComponent.IsPaused = false;

            EcsEntity? effectiveTarget;
            if (target != null)
            {
                SetCurrentTarget(entity, target);
                effectiveTarget = target;
            }
            else
            {
                effectiveTarget = GetCurrentTarget(entity);
            }

            if (effectiveTarget == null)
            {
                ClearCurrentTarget(entity);
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
            var chaseComponent = entity.GetComponent<ChaseComponent>();
            if (chaseComponent.IsPaused)
            {
                return;
            }

            if (!HasActiveTarget(entity))
            {
                ChaseStateSystem.SetState(entity, ChaseState.Searching);
                ChaseStateSystem.SetCurrentDistance(entity, 0f);
                var idleKinematics = new ChaseKinematics
                {
                    Direction = Vector3.zero,
                    Speed = 0f
                };
                ChaseStateSystem.SetKinematics(entity, in idleKinematics);
                return;
            }

            var snapshot = CaptureRuntime(entity, chaseComponent.CurrentTargetId);
            if (!snapshot.HasTarget)
            {
                HandleLostTarget(entity, chaseComponent.CurrentTargetId);
                return;
            }

            UpdateTrackingState(entity, in snapshot, deltaTime);
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
            ConsoleLog.Debug($"UpdateTarget: EntityId={entity.Id}, OldTargetId={oldId}, NewTargetId={newId}");
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
            ChaseStateSystem.SetState(entity, ChaseState.Searching);
        }

        private static void UpdateTrackingState(EcsEntity entity, in RuntimeSnapshot snapshot, float deltaTime)
        {
            var previousDistance = ChaseStateSystem.GetCurrentDistance(entity);
            var distance = snapshot.Distance;
            ChaseStateSystem.SetCurrentDistance(entity, distance);

            var speed = deltaTime > 0f ? Mathf.Abs(distance - previousDistance) / deltaTime : 0f;
            var kinematics = new ChaseKinematics
            {
                Direction = snapshot.Direction,
                Speed = speed
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
            if (targetId == 0)
            {
                return null;
            }

            var world = FindWorld(entity);
            if (world == null)
            {
                return null;
            }


            var target = ActorListSystem.GetActor(world, targetId);
            ConsoleLog.Debug($"ResolveTarget: EntityId={entity.Id}, TargetId={targetId}, Found={(target != null)}");
            return target;
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

        private static RuntimeSnapshot CaptureRuntime(EcsEntity entity, long targetId)
        {
            var snapshot = new RuntimeSnapshot
            {
                OwnerPosition = TransformSystem.GetPosition(entity).ToVector(),
                HasOwner = true
            };

            if (targetId <= 0)
            {
                return snapshot;
            }

            var target = ResolveTarget(entity, targetId);
            if (target == null)
            {
                return snapshot;
            }

            snapshot.TargetPosition = TransformSystem.GetPosition(target).ToVector();
            snapshot.HasTarget = true;
            return snapshot;
        }
    }
}
