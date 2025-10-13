using System;

namespace ECSGame.AttackModule
{
    /// <summary>
    /// 攻击时间轴阶段。
    /// </summary>
    /// <remarks>
    /// 用于表征一次攻击在生命周期中的所处阶段，常见顺序：
    /// None → Windup → Active → Recovery → Ended。
    /// 其中 <see cref="Canceled"/> 可在任意尚未结束的阶段发生，并与 <see cref="Ended"/>（自然结束）区分。
    /// 注意：枚举取值属于协议/存档稳定值，请勿调整既有项的顺序与数值。
    /// </remarks>
    public enum AttackPhase
    {
        /// <summary>
        /// 未处于任何攻击阶段（默认值）。用于初始化或尚未进入起手阶段。
        /// </summary>
        None = 0,

        /// <summary>
        /// 起手/准备阶段（Windup）。常见为前摇、蓄力、对齐朝向/目标采样等；通常尚未产生伤害判定。
        /// </summary>
        Windup = 1,

        /// <summary>
        /// 生效阶段（Active）。命中盒/判定帧有效，可产生伤害与效果，可能包含多段命中。
        /// </summary>
        Active = 2,

        /// <summary>
        /// 后摇/收招阶段（Recovery）。伤害一般已结束，进入动作收尾与冷却窗口，仍可能受打断影响。
        /// </summary>
        Recovery = 3,

        /// <summary>
        /// 自然结束。按正常流程完成所有阶段后结束，区别于 <see cref="Canceled"/>。
        /// </summary>
        Ended = 4,

        /// <summary>
        /// 被取消结束。攻击在进行中被提前终止；具体原因见 <see cref="AttackCancelReason"/>。
        /// </summary>
        Canceled = 5
    }
}
