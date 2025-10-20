using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供追踪配置读写的系统。</summary>
    public partial class ChaseConfigSystem : AComponentSystem<EcsEntity, ChaseConfigComponent>
    {
        /// <summary>读取追踪配置。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>追踪配置实例。</returns>
        public static IChaseConfig GetConfig(EcsEntity entity)
        {
            return entity.GetComponent<ChaseConfigComponent>().Config!;
        }

        /// <summary>写入追踪配置。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="config">追踪配置实例。</param>
        public static void SetConfig(EcsEntity entity, IChaseConfig config)
        {
            entity.GetComponent<ChaseConfigComponent>().Config = config;
        }
    }
}
