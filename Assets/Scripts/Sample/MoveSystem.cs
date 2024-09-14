using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ABeDrivedEntitySystem2<EcsAwakeDriveSystem, Actor, MoveComponent>, 
public class MoveSystem : AEcsSystem2<Actor, MoveComponent>,
    IAwake<Actor, MoveComponent>,
    IInit<Actor, MoveComponent>,
    IUpdate<Actor, MoveComponent>
{
    public void Awake(Actor actor, MoveComponent moveComponent)
    {
        Debug.Log($"MoveSystem Awake {actor.GetType().Name} {moveComponent.GetType().Name}");
        foreach (var item in actor.components)
        {
            Debug.Log($"MoveSystem Awake {item.Key.Name}");
        }
    }

    public void Init(Actor actor, MoveComponent moveComponent)
    {
        Debug.Log($"MoveSystem Init {actor.GetType().Name} {moveComponent.GetType().Name}");
        foreach (var item in actor.components)
        {
            Debug.Log($"MoveSystem Init {item.Key.Name}");
        }
    }

    public void Update(Actor actor, MoveComponent moveComponent)
    {
        Debug.Log($"MoveSystem Update {actor.GetType().Name} {moveComponent.GetType().Name}");
    }
}
