using ECS;
using System.Collections;
using System.Collections.Generic;

public class HealthSystem : AEcsComponentSystem<Actor, HealthComponent>,
    IAwake<Actor, HealthComponent>
{
    public void Awake(Actor entity, HealthComponent component)
    {
        Debug.Log($"HealthSystem Awake {entity.GetType().Name}");
    }
}
