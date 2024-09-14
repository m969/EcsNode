using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public abstract class AEcsSystem<TEntity> : IEcsEntitySystem where TEntity : EcsEntity
    {
        public Type EntityType { get => typeof(TEntity); }
    }

    public abstract class AEcsSystem2<TEntity, TComponent> : IEcsEntitySystem2 where TEntity : EcsEntity where TComponent : IEcsComponent
    {
        public Type EntityType { get => typeof(TEntity); }
        public Type ComponentType { get => typeof(TComponent); }
    }
}
