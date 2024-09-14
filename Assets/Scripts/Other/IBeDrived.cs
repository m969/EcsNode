
using System;

namespace ECS
{
    //public interface IBeDrived
    //{
    //    Type DriveSystemType { get; }
    //}

    //public abstract class ABeDrivedEntitySystem1<TSystem, TEntity> : IEcsEntitySystem1, IBeDrived where TEntity : EcsEntity where TSystem : IEcsDriveSystem
    //{
    //    public Type DriveSystemType { get => typeof(TSystem); }
    //    public Type EntityType { get => typeof(TEntity); }

    //    public void InvokeHandle(EcsNode ecsNode, object entity)
    //    {
    //        Handle(ecsNode, entity as TEntity);
    //    }

    //    public abstract void Handle(EcsNode ecsNode, TEntity entity);
    //}

    //public abstract class ABeDrivedEntitySystem2<TSystem, TEntity, TComponent> : IEcsEntitySystem2, IBeDrived where TSystem : IEcsDriveSystem where TEntity : EcsEntity where TComponent : class, IEcsComponent
    //{
    //    public Type DriveSystemType { get => typeof(TSystem); }
    //    public Type EntityType { get => typeof(TEntity); }

    //    public abstract void Handle(EcsNode ecsNode, TEntity entity, TComponent component);

    //    public void InvokeHandle(EcsNode ecsNode, object entity, object component)
    //    {
    //        Handle(ecsNode, entity as TEntity, component as TComponent);
    //    }
    //}
}