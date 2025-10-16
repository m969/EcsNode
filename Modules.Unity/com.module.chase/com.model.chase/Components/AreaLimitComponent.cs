using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>保存追踪区域限制以及越界标记。</summary>
    public class AreaLimitComponent : EcsComponent
    {
        /// <summary>区域限制配置。</summary>
        public IChaseAreaConfig? Area { get; set; }

        /// <summary>是否当前处于越界状态。</summary>
        public bool IsOutOfArea { get; set; }
    }
}
