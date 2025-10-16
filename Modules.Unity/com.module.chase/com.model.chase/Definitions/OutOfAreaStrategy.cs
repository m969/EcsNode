namespace ECSGame.ChaseModule
{
    /// <summary>定义追踪实体越界时的处理策略。</summary>
    public enum OutOfAreaStrategy
    {
        /// <summary>越界后不做处理。</summary>
        None,

        /// <summary>越界时回退至安全区域。</summary>
        Rollback,

        /// <summary>越界时停止追踪。</summary>
        StopChase
    }
}
