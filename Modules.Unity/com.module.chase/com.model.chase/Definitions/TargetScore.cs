namespace ECSGame.ChaseModule
{
    /// <summary>记录候选目标对应的评分。</summary>
    public struct TargetScore
    {
        /// <summary>候选目标的实体 Id。</summary>
        public long TargetId { get; set; }

        /// <summary>该目标计算得到的评分。</summary>
        public float Score { get; set; }
    }
}
