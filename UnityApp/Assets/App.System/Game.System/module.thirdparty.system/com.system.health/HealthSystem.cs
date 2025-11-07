using ECS;
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
        public static void AddOn(Actor actor, int health)
        {
            var healthComp = actor.AddComponent<HealthComponent>(beforeAwake: (comp) =>
            {
                comp.Health = health;
                comp.MaxHealth = health;
            });
        }

        public void Awake(Actor entity, HealthComponent component)
        {
            //ConsoleLog.Debug($"HealthSystem Awake {entity.GetType().Name}");
        }

        public static void ChangeHealth(Actor entity, int value)
        {
            var healthComp = entity.GetComponent<HealthComponent>();
            healthComp.Health += value;
            if (healthComp.Health < 0)
            {
                healthComp.Health = 0;
            }
            if (healthComp.Health > healthComp.MaxHealth)
            {
                healthComp.Health = healthComp.MaxHealth;
            }

            entity.Dispatch<IHealthChangeHandler>((anySystem) => anySystem.OnHealthChange(entity, healthComp));
        }
    }
}
