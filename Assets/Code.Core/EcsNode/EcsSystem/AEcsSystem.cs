using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public abstract class AEcsSystem<TEntity> : IEcsSystem1 where TEntity : EcsEntity
    {
        public Type EntityType { get => typeof(TEntity); }
    }

    public abstract class AEcsSystem2<TEntity, TComponent> : IEcsSystem2 where TEntity : EcsEntity where TComponent : EcsComponent
    {
        public Type EntityType { get => typeof(TEntity); }
        public Type ComponentType { get => typeof(TComponent); }
    }
}
