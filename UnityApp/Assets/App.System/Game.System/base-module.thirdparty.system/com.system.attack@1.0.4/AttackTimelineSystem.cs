using ECS;
using System;

namespace ECSGame.AttackModule
{
    // 组件系统：时间轴推进（仅静态业务方法）
    public class AttackTimelineSystem : AComponentSystem<AttackAction, AttackTimelineComponent>
    {
        /// <summary>
        /// 从配置初始化时间轴数据
        /// </summary>
        public static void InitFromConfig(AttackAction action, IAttackConfig cfg, long nowMs)
        {
            var tl = action.GetComponent<AttackTimelineComponent>();
            tl.WindupDurationMs = cfg.WindupDurationMs;
            tl.ActiveDurationMs = cfg.ActiveDurationMs;
            tl.RecoveryDurationMs = cfg.RecoveryDurationMs;
            tl.CreatedTimeMs = nowMs;
            tl.CurrentPhaseStartMs = nowMs;
            tl.CurrentPhase = AttackPhase.Windup;
            tl.HitEmitted = false;
            tl.HasHit = false;
            tl.CancelReason = AttackCancelReason.None;
        }

        /// <summary>
        /// 尝试进入下一阶段
        /// </summary>
        public static bool EnterNextPhase(AttackAction action, long nowMs)
        {
            var tl = action.GetComponent<AttackTimelineComponent>();
            switch (tl.CurrentPhase)
            {
                case AttackPhase.Windup:
                    tl.CurrentPhase = AttackPhase.Active;
                    tl.CurrentPhaseStartMs = nowMs;
                    return true;
                case AttackPhase.Active:
                    tl.CurrentPhase = AttackPhase.Recovery;
                    tl.CurrentPhaseStartMs = nowMs;
                    return true;
                case AttackPhase.Recovery:
                    tl.CurrentPhase = AttackPhase.Ended;
                    tl.CurrentPhaseStartMs = nowMs;
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 推进并在到达阈值时切换阶段
        /// </summary>
        public static void TickAndTransit(AttackAction action, long nowMs)
        {
            var tl = action.GetComponent<AttackTimelineComponent>();
            var elapsed = (int)Math.Max(0, nowMs - tl.CurrentPhaseStartMs);
            switch (tl.CurrentPhase)
            {
                case AttackPhase.Windup:
                    if (elapsed >= tl.WindupDurationMs)
                        EnterNextPhase(action, nowMs);
                    break;
                case AttackPhase.Active:
                    if (elapsed >= tl.ActiveDurationMs)
                        EnterNextPhase(action, nowMs);
                    break;
                case AttackPhase.Recovery:
                    if (elapsed >= tl.RecoveryDurationMs)
                        EnterNextPhase(action, nowMs);
                    break;
            }
        }
    }
}
