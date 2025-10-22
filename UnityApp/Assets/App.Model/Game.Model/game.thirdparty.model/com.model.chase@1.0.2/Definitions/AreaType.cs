namespace ECSGame.ChaseModule
{
    /// <summary>表示追踪限制所采用的区域形状。</summary>
    public enum AreaType
    {
        /// <summary>不限制区域。</summary>
        None,

        /// <summary>使用圆形区域。</summary>
        Circle,

        /// <summary>使用多边形区域。</summary>
        Polygon
    }
}
