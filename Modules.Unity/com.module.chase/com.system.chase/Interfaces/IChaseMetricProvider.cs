using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>提供目标评分数据的接口。</summary>
    public interface IChaseMetricProvider : IDispatch
    {
        /// <summary>获取与目标的距离。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标实体 Id。</param>
        /// <returns>距离标量。</returns>
        float GetDistance(EcsEntity entity, long targetId);

        /// <summary>获取自定义评分指标值。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标实体 Id。</param>
        /// <param name="metricKey">指标键。</param>
        /// <returns>指标对应的值。</returns>
        float GetMetric(EcsEntity entity, long targetId, string metricKey);
    }
}
