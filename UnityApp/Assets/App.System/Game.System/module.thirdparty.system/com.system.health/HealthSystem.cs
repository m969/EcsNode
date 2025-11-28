using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public interface IHealthChangeHandler : IDispatch
    {
        void OnHealthChange(Actor entity, HealthComponent component);
    }

    public class HealthSystem : AComponentSystem<Actor, HealthComponent>,
IAwake<Actor, HealthComponent>
    {
        public void Awake(Actor entity, HealthComponent component)
        {
            //ConsoleLog.Debug($"HealthSystem Awake {entity.GetType().Name}");
        }

        public static int GetHealth(Actor entity)
        {
            var healthComp = entity.GetComponent<HealthComponent>();
            return healthComp.Health;
        }

        public static void ChangeHealth(Actor entity, int value)
        {
            ConsoleLog.Error($"HealthSystem ChangeHealth {entity.Id} {value}");
            var healthComp = entity.GetComponent<HealthComponent>();
            healthComp.Health = Math.Clamp(healthComp.Health + value, 0, healthComp.MaxHealth);
            entity.Dispatch<IHealthChangeHandler>((anySystem) => anySystem.OnHealthChange(entity, healthComp));
        }
    }
}
