using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供追踪系统时间上下文的接口。</summary>
    public interface IChaseTimeProvider : IDispatch
    {
        /// <summary>返回当前时间戳。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>当前时间。</returns>
        float GetTime(EcsEntity entity);
    }
}
