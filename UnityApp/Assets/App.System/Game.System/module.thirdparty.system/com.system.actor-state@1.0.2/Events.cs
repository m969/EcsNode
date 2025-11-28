using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 状态进入时派发
    /// </summary>
    public interface IStateEnterHandler : IDispatch
    {
        void OnStateEnter(EcsEntity entity, ActorStateType stateType);
    }

    /// <summary>
    /// 状态退出时派发
    /// </summary>
    public interface IStateExitHandler : IDispatch
    {
        void OnStateExit(EcsEntity entity, ActorStateType stateType);
    }

    /// <summary>
    /// 状态更新时派发
    /// </summary>
    public interface IStateUpdateHandler : IDispatch
    {
        void OnStateUpdate(EcsEntity entity);
    }
}
