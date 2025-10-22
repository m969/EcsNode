using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>缓存追踪配置供系统快速读取。</summary>
    public class ChaseConfigComponent : EcsComponent
    {
        /// <summary>绑定的追踪配置对象。</summary>
        public IChaseConfig? Config { get; set; }
    }
}
