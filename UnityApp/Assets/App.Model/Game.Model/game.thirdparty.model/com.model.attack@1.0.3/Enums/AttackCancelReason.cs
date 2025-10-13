using System;

namespace ECSGame.AttackModule
{
    /// <summary>
    /// 表示攻击被取消的原因。
    /// </summary>
    /// <remarks>
    /// 本枚举用于系统间传递取消状态，请勿修改已有枚举值与数值，以避免引发序列化或网络协议不兼容问题。
    /// </remarks>
    public enum AttackCancelReason
    {
        /// <summary>
        /// 未被取消（默认值）。
        /// </summary>
        None = 0,

        /// <summary>
        /// 攻击被外因打断，例如：硬直、击退、控制效果、被其他系统强制中断等。
        /// </summary>
        Interrupted = 1,

        /// <summary>
        /// 由操作者或系统逻辑主动取消，例如：玩家手动取消、AI策略切换、显式调用取消接口等。
        /// </summary>
        Manual = 2,

        /// <summary>
        /// 目标无效导致取消，例如：目标死亡、超出范围、失去可见性、引用丢失或不再满足命中层/阵营校验等。
        /// </summary>
        InvalidTarget = 3
    }
}
