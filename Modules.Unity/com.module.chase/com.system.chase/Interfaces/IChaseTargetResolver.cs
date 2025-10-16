using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供将目标 Id 解析为实体实例的接口。</summary>
    public interface IChaseTargetResolver : IDispatch
    {
        /// <summary>解析目标实体。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标 Id。</param>
        /// <returns>解析得到的目标实体。</returns>
        EcsEntity? Resolve(EcsEntity entity, long targetId);
    }
}
