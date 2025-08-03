using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成类型枚举
    /// </summary>
    public enum AchieveType
    {
        /// <summary>一次性达成</summary>
        OneTime,
        /// <summary>重复性达成</summary>
        Repeatable,
        /// <summary>连锁性达成</summary>
        Chain,
        /// <summary>隐藏性达成</summary>
        Hidden
    }

    /// <summary>
    /// 达成状态枚举
    /// </summary>
    public enum AchieveStatus
    {
        /// <summary>未开始</summary>
        NotStarted,
        /// <summary>进行中</summary>
        InProgress,
        /// <summary>已完成</summary>
        Completed,
        /// <summary>已过期</summary>
        Expired
    }

    /// <summary>
    /// 条件逻辑类型枚举
    /// </summary>
    public enum ConditionLogicType
    {
        /// <summary>全部满足</summary>
        AllSatisfied,
        /// <summary>任一满足</summary>
        AnySatisfied,
        /// <summary>权重计算</summary>
        WeightCalculation
    }

    /// <summary>
    /// 奖励类型枚举
    /// </summary>
    public enum RewardType
    {
        /// <summary>虚拟货币</summary>
        Currency,
        /// <summary>道具</summary>
        Item,
        /// <summary>装扮</summary>
        Costume,
        /// <summary>称号</summary>
        Title,
        /// <summary>特权</summary>
        Privilege
    }

    /// <summary>
    /// 奖励状态枚举
    /// </summary>
    public enum RewardStatus
    {
        /// <summary>未发放</summary>
        NotGranted,
        /// <summary>已发放</summary>
        Granted,
        /// <summary>发放失败</summary>
        Failed
    }

    /// <summary>
    /// 发放方式枚举
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>即时发放</summary>
        Immediate,
        /// <summary>延迟发放</summary>
        Delayed,
        /// <summary>批量发放</summary>
        Batch
    }

    /// <summary>
    /// 同步状态枚举
    /// </summary>
    public enum SyncStatus
    {
        /// <summary>本地状态</summary>
        Local,
        /// <summary>已同步</summary>
        Synced,
        /// <summary>待同步</summary>
        PendingSync
    }
}
