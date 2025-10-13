using System;

namespace ECSGame.AttackModule
{
    /// <summary>
    /// 伤害计算公式的类型定义。
    /// </summary>
    /// <remarks>
    /// 仅描述计算框架，不包含具体参数；参数通常来自配置或上下文（如攻击力、系数等）。
    /// 枚举取值作为协议/存档的稳定值，请勿调整既有项的顺序与数值。
    /// </remarks>
    public enum DamageFormulaType
    {
        /// <summary>
        /// 固定值：伤害 = 常量值。
        /// 常用于固定伤害、环境伤害或触发类效果。
        /// </summary>
        Flat = 0,

        /// <summary>
        /// 差值型：伤害 = 攻击 - 防御。
        /// 具体是否有下限/上限/系数修正由实现或配置决定。
        /// </summary>
        AttackMinusDefense = 1,

        /// <summary>
        /// 比例型：伤害 = 攻击 × 比例。
        /// 比例通常由配置提供，可能叠加其他修正项。
        /// </summary>
        AttackTimesRatio = 2
    }
}
