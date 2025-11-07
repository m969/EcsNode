using ECS;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 单次派遣执行实体（UnitDispatcher）：作为独立执行体附着在宿主实体上，承载派遣次数、目标与超时等数据。
    /// 仅承载数据属性，不包含业务逻辑。
    /// </summary>
    public class UnitDispatcher : EcsEntity
    {
        /// <summary>配置 Id，引用外部 UnitDispatchConfig</summary>
        public int ConfigId { get; set; }

        /// <summary>本次执行累计派遣数量</summary>
        public int DispatchCount { get; set; }

        /// <summary>目标实体 Id</summary>
        public long TargetEntityId { get; set; }

        /// <summary>超时（秒），0 或负数 表示未启用</summary>
        public float Timeout { get; set; }
    }
}
