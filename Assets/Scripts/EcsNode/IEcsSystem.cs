using System;

namespace ECS
{
    public interface IEcsDriveSystem
    {
        
    }

    public interface IEcsDriveSystem<in T> : IEcsDriveSystem where T : EcsEntity
    {
        void Handle(EcsNode ecsNode, T entity);
    }

    /// <summary>
    /// 系统是包含的机制，大系统包含小系统，小系统包含更小的系统
    /// </summary>
    public interface IEcsEntitySystem
    {
        Type EntityType { get; }
    }

    public interface IEcsEntitySystem2 : IEcsEntitySystem
    {
        Type ComponentType { get; }
    }

    //public interface IEcsEntitySystem1 : IEcsEntitySystem
    //{
    //    void InvokeHandle(EcsNode ecsNode, object entity);
    //}

    //public interface IEcsEntitySystem2 : IEcsEntitySystem
    //{
    //    void InvokeHandle(EcsNode ecsNode, object entity, object component);
    //}

    //public abstract class AEcsEntitySystem<TEntity, TComponent> : IEcsEntitySystem2 where TEntity : EcsEntity where TComponent : EcsEntity, IEcsComponent
    //{
    //    public Type EntityType { get => typeof(TEntity); }

    //    public abstract void Handle(EcsNode ecsNode, TEntity entity, TComponent component);

    //    public void InvokeHandle(EcsNode ecsNode, object entity, object component)
    //    {
    //        Handle(ecsNode, entity as TEntity, component as TComponent);
    //    }
    //}
}