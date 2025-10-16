namespace ECSGame.ChaseModule
{
    /// <summary>指定候选目标评分策略类型。</summary>
    public enum PriorityPolicy
    {
        /// <summary>按距离升序选择。</summary>
        DistanceAsc,

        /// <summary>使用自定义权重规则。</summary>
        Custom
    }
}
