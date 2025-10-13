using System;

namespace ECSGame.AttackModule
{
    /// <summary>
    /// 伤害计算失败（无法得出有效伤害值）的原因分类。
    /// </summary>
    /// <remarks>
    /// 适用于伤害公式执行阶段的失败分类，区别于攻击启动或取消阶段的错误分类。
    /// 枚举取值为协议/存档稳定值，请勿调整既有项的顺序与数值。
    /// </remarks>
    public enum DamageFailReason
    {
        /// <summary>
        /// 未失败（默认值）。用于初始化或表示未发生错误的占位。
        /// </summary>
        None = 0,

        /// <summary>
        /// 缺少必需属性/参数，例如缺失攻击力、防御力、比例参数等导致无法计算。
        /// </summary>
        AttributeMissing = 1,

        /// <summary>
        /// 计算过程中发生数值溢出/下溢，或超过实现允许的范围。
        /// </summary>
        Overflow = 2,

        /// <summary>
        /// 不支持的公式类型或当前实现未覆盖的计算分支。
        /// </summary>
        FormulaUnsupported = 3,

        /// <summary>
        /// 未分类或不可预期的内部错误，建议记录详细日志以便排查。
        /// </summary>
        InternalError = 99
    }
}
