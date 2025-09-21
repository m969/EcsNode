using ECS;

namespace ECSGame.TaskModule
{
    public interface IOnTaskActivated : IDispatch
    {
        void OnTaskActivated(EcsEntity entity, int taskId);
    }

    public interface IOnTaskProgressChanged : IDispatch
    {
        void OnTaskProgressChanged(EcsEntity entity, int taskId, float progress);
    }

    public interface IOnTaskCompleted : IDispatch
    {
        void OnTaskCompleted(EcsEntity entity, int taskId);
    }

    public interface IOnTaskRewardClaimed : IDispatch
    {
        void OnTaskRewardClaimed(EcsEntity entity, int taskId);
    }

    public interface IOnTaskReset : IDispatch
    {
        void OnTaskReset(EcsEntity entity, int taskId);
    }
}
