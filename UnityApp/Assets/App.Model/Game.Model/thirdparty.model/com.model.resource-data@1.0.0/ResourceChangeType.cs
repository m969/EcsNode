namespace ECSGame.Module.ResourceData
{
    /// <summary>
    /// 资源变更类型枚举
    /// </summary>
    public enum ResourceChangeType
    {
        /// <summary>
        /// 获得资源
        /// </summary>
        Gain,
        /// <summary>
        /// 消耗资源
        /// </summary>
        Consume,
        /// <summary>
        /// 奖励资源
        /// </summary>
        Reward,
        /// <summary>
        /// 扣除资源
        /// </summary>
        Deduct,
        // 可扩展更多类型
    }
}
