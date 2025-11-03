namespace ECS
{
    public interface IDispatch
    {

    }

    public interface IEventDispatch<T> : IDispatch where T : IEvent
    {
        void OnHandleEvent(EcsNode ecsNode, T inputEvent);
    }
}