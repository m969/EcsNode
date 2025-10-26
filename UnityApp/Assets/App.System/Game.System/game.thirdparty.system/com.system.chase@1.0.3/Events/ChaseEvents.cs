using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>追踪开始事件派发接口。</summary>
    public interface IOnChaseStarted : IDispatch
    {
        /// <summary>当追踪流程开始时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="target">初始目标实体。</param>
        void OnChaseStarted(EcsEntity entity, EcsEntity? target);
    }

    /// <summary>追踪暂停事件派发接口。</summary>
    public interface IOnChasePaused : IDispatch
    {
        /// <summary>当追踪流程被暂停时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        void OnChasePaused(EcsEntity entity);
    }

    /// <summary>追踪恢复事件派发接口。</summary>
    public interface IOnChaseResumed : IDispatch
    {
        /// <summary>当追踪流程恢复时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        void OnChaseResumed(EcsEntity entity);
    }

    /// <summary>追踪停止事件派发接口。</summary>
    public interface IOnChaseStopped : IDispatch
    {
        /// <summary>当追踪流程结束时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="reason">停止原因。</param>
        void OnChaseStopped(EcsEntity entity, string reason);
    }

    /// <summary>追踪目标变更事件派发接口。</summary>
    public interface IOnChaseTargetChanged : IDispatch
    {
        /// <summary>当追踪目标发生变更时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="oldTarget">旧目标实体。</param>
        /// <param name="newTarget">新目标实体。</param>
        void OnChaseTargetChanged(EcsEntity entity, EcsEntity? oldTarget, EcsEntity? newTarget);
    }

    /// <summary>进入判定半径事件派发接口。</summary>
    public interface IOnChaseEnterRadius : IDispatch
    {
        /// <summary>当目标进入判定半径时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="target">当前目标。</param>
        /// <param name="radius">触发使用的半径阈值。</param>
        void OnChaseEnterRadius(EcsEntity entity, EcsEntity target, float radius);
    }

    /// <summary>保持距离达成事件派发接口。</summary>
    public interface IOnChaseKeepDistanceReached : IDispatch
    {
        /// <summary>当达到保持距离阈值时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="target">当前目标。</param>
        /// <param name="keepDistance">保持距离阈值。</param>
        void OnChaseKeepDistanceReached(EcsEntity entity, EcsEntity target, float keepDistance);
    }

    /// <summary>目标丢失事件派发接口。</summary>
    public interface IOnChaseLostTarget : IDispatch
    {
        /// <summary>当追踪目标丢失时触发。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="lastTarget">丢失前的目标。</param>
        void OnChaseLostTarget(EcsEntity entity, EcsEntity? lastTarget);
    }
}
