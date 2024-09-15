using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySystem : AEcsSystem<EcsEntity>,
    IAwake<EcsEntity>
{
    public void Awake(EcsEntity entity)
    {
        Debug.Log($"EntitySystem Awake {entity.GetType().Name}");
    }
}
