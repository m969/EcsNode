using System;

namespace ECSGame.AttackModule
{
    /// <summary>
    /// 攻击启动失败（尚未进入正常执行流程）的原因分类。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="AttackCancelReason"/> 的区别：失败发生在“尚未开始/未成功创建攻击流程”阶段；
    /// 取消则是“已开始但未按预期完成”。
    /// 枚举取值为协议/存档稳定值，请勿调整既有项的顺序与数值。
    /// </remarks>
    public enum AttackFailureReason
    {
        /// <summary>
        /// 未失败（默认值）。通常用于初始化或无错误场景的占位。
        /// </summary>
        None = 0,

        /// <summary>
        /// 发起者无效，例如：实体不存在、未激活、缺少关键组件或状态不允许发起攻击。
        /// </summary>
        InvalidAttacker = 1,

        /// <summary>
        /// 目标不合法或不可选，例如：阵营/层级校验失败、目标不存在/死亡、不可被选中。
        /// </summary>
        InvalidTarget = 2,

        /// <summary>
        /// 未找到攻击配置，可能是配置ID无效、表未加载或提供者返回空。
        /// </summary>
        ConfigNotFound = 3,

        /// <summary>
        /// 时机不合法，例如：冷却未结束、前置条件窗口未到、时间轴不匹配。
        /// </summary>
        TimeInvalid = 4,

        /// <summary>
        /// 正在进行其他互斥的攻击或动作，无法并发启动当前攻击。
        /// </summary>
        AlreadyAttacking = 5,

        /// <summary>
        /// 未分类或不可预期的内部错误，用作兜底。建议记录详细日志以便排查。
        /// </summary>
        InternalError = 99
    }
}
