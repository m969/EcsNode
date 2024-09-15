using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MoveSystem : AEcsSystem2<Actor, MoveComponent>,
    IAwake<Actor, MoveComponent>,
    IInit<Actor, MoveComponent>,
    IUpdate<Actor, MoveComponent>
{
    public void Awake(Actor actor, MoveComponent moveComponent)
    {
        Debug.Log($"MoveSystem Awake {actor.GetType().Name} {moveComponent.GetType().Name}");
    }

    public void Init(Actor actor, MoveComponent moveComponent)
    {
        Debug.Log($"MoveSystem Init {actor.GetType().Name} {moveComponent.GetType().Name}");
    }

    public void Update(Actor actor, MoveComponent moveComponent)
    {
        //Debug.Log($"MoveSystem Update {actor.GetType().Name} {moveComponent.GetType().Name}");
    }
}

public static class MoveSystemExtension
{
    public static async Task MoveToAsync(this Actor actor, Vector3 target)
    {
        var moveComp = actor.GetComponent<MoveComponent>();
    }
}