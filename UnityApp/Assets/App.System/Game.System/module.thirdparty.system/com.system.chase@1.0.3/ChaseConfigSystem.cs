using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供追踪配置读写的系统。</summary>
    public partial class ChaseConfigSystem : AComponentSystem<EcsEntity, ChaseConfigComponent>
    {
        /// <summary>读取追踪配置组件。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>追踪配置组件。</returns>
        public static ChaseConfigComponent GetConfig(EcsEntity entity)
        {
            return entity.GetComponent<ChaseConfigComponent>();
        }

        /// <summary>写入追踪配置。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="configId">配置标识。</param>
        /// <param name="enterRadius">进入判定半径。</param>
        /// <param name="exitRadius">退出判定半径。</param>
        /// <param name="keepDistance">保持距离阈值。</param>
        /// <param name="lostDistance">丢失距离阈值。</param>
        public static void SetConfig(EcsEntity entity, string configId, float enterRadius, float exitRadius, float keepDistance, float lostDistance)
        {
            var component = entity.GetComponent<ChaseConfigComponent>();
            component.ConfigId = configId;
            component.EnterRadius = enterRadius;
            component.ExitRadius = exitRadius;
            component.KeepDistance = keepDistance;
            component.LostDistance = lostDistance;
        }
    }
}
