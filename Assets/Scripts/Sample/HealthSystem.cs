using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : AEcsSystem2<Actor, HealthComponent>,
    IAwake<Actor, HealthComponent>
{
    public void Awake(Actor entity, HealthComponent component)
    {
        Debug.Log($"HealthSystem Awake {entity.GetType().Name}");
    }
}
