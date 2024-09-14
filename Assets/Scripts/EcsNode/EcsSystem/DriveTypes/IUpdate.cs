using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public interface IUpdate
    {
    }

    public interface ISecondUpdate
    {
    }

    public interface IUpdate<T, T2> : IUpdate where T : EcsEntity where T2 : IEcsComponent
    {
        void Update(T entity, T2 component);
    }
}
