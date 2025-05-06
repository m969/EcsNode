using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class HealthSystem : AComponentSystem<Actor, HealthComponent>,
IAwake<Actor, HealthComponent>
    {
        public void Awake(Actor entity, HealthComponent component)
        {
            //ConsoleLog.Debug($"HealthSystem Awake {entity.GetType().Name}");
        }
    } 
}
