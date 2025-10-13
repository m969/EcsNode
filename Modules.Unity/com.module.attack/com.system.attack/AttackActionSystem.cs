using ECS;
using System;

namespace ECSGame.AttackModule
{
    // 实体系统：单次普攻驱动（静态业务方法）
    public class AttackActionSystem : AEntitySystem<AttackAction>
    {
        /// <summary>
        /// 推进单次普攻流程：时间轴推进、命中一次性触发、结束收尾
        /// </summary>
        public static void Tick(AttackAction action, long nowMs)
        {
            var tl = action.GetComponent<AttackTimelineComponent>();
            AttackTimelineSystem.TickAndTransit(action, nowMs);

            if (tl.CurrentPhase == AttackPhase.Active && !tl.HitEmitted)
            {
                if (AttackDamageSystem.TryCalculateDamage(action, out var finalDamage, out var fail))
                {
                    tl.HitEmitted = true;
                    action.HasHit = true;
                    action.Dispatch<IOnAttackHit>(sys => sys.OnAttackHit(action, action, action.AttackerId, action.TargetId, finalDamage));
                }
                else
                {
                    // 计算失败不触发命中事件，可由外部在配置层避免
                    tl.HitEmitted = true; // 确保只尝试一次
                }
            }

            if (tl.CurrentPhase == AttackPhase.Ended)
            {
                action.Dispatch<IOnAttackEnd>(sys => sys.OnAttackEnd(action, action));
            }
        }

        public static bool TryStartAttack(EcsEntity actor, long targetId, long nowMs, int configId, out AttackFailureReason fail, out long newActionId)
        {
            // 基础校验
            if (targetId <= 0)
            {
                fail = AttackFailureReason.InvalidTarget;
                newActionId = 0;
                return false;
            }
            if (nowMs < 0)
            {
                fail = AttackFailureReason.TimeInvalid;
                newActionId = 0;
                return false;
            }

            // 查询配置
            IAttackConfig? cfg = null;
            actor.Dispatch<IAttackConfigProvider>(p => cfg = p.GetConfig(actor, configId));
            if (cfg == null)
            {
                fail = AttackFailureReason.ConfigNotFound;
                newActionId = 0;
                return false;
            }

            // 并发限制（可选）
            var list = actor.GetComponent<AttackListComponent>();
            if (list != null && list.AttackerId2LatestAttackId.TryGetValue(actor.Id, out var latestId))
            {
                if (latestId != 0 && AttackListSystem.Get(actor, latestId) != null)
                {
                    fail = AttackFailureReason.AlreadyAttacking;
                    newActionId = 0;
                    return false;
                }
            }

            // 创建上下文与实体
            var ctx = new AttackContext
            {
                AttackerId = actor.Id,
                TargetId = targetId,
                ConfigId = configId,
                StartTimeMs = nowMs
            };

            var action = AttackListSystem.Create(actor, in ctx, cfg);

            // 初始化组件
            action.AddComponent<AttackTimelineComponent>(_ => { });
            AttackTimelineSystem.InitFromConfig(action, cfg, nowMs);

            action.AddComponent<AttackDamageComponent>(c =>
            {
                c.BaseDamage = cfg.BaseDamage;
                c.Formula = cfg.DamageFormula;
            });

            // 派发开始事件
            actor.Dispatch<IOnAttackStart>(sys => sys.OnAttackStart(actor, action));

            fail = AttackFailureReason.None;
            newActionId = action.Id;
            return true;
        }

        /// <summary>
        /// 取消该普攻
        /// </summary>
        public static bool TryCancelAttack(AttackAction action, AttackCancelReason reason)
        {
            var tl = action.GetComponent<AttackTimelineComponent>();
            if (tl.CurrentPhase == AttackPhase.Ended || tl.CurrentPhase == AttackPhase.Canceled)
                return false;

            action.CancelReason = reason;
            tl.CurrentPhase = AttackPhase.Canceled;
            tl.CurrentPhaseStartMs = tl.CurrentPhaseStartMs; // 维持原始时间点

            action.Dispatch<IOnAttackCancel>(sys => sys.OnAttackCancel(action, action, reason));
            return true;
        }
    }
}
