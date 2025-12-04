using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 状态进入时派发
    /// </summary>
    public interface IStateEnterHandler : IDispatch
    {
        void OnStateEnterHandle(EcsEntity entity, ActorStateType stateType);
    }

    /// <summary>
    /// 状态退出时派发
    /// </summary>
    public interface IStateExitHandler : IDispatch
    {
        void OnStateExitHandle(EcsEntity entity, ActorStateType stateType);
    }

    /// <summary>
    /// 状态更新时派发
    /// </summary>
    public interface IStateUpdateHandler : IDispatch
    {
        void OnStateUpdateHandle(EcsEntity entity);
    }
}
