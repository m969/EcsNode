using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>缓存追踪配置供系统快速读取。</summary>
    public class ChaseConfigComponent : EcsComponent
    {
        /// <summary>配置标识。</summary>
        public string ConfigId { get; set; } = string.Empty;

        /// <summary>进入判定半径。</summary>
        public float EnterRadius { get; set; }

        /// <summary>退出判定半径。</summary>
        public float ExitRadius { get; set; }

        /// <summary>保持距离阈值。</summary>
        public float KeepDistance { get; set; }

        /// <summary>丢失距离阈值。</summary>
        public float LostDistance { get; set; }
    }
}
