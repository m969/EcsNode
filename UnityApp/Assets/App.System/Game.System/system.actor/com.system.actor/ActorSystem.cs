using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using ECSGame.TaskModule;

namespace ECSGame
{
    public class ActorType
    {
        public const int Hero = 1;
        public const int Monster = 2;
    }

    public class ActorSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IHealthChangeHandler
    {
        public void Awake(Actor entity)
        {

        }

        public void Init(Actor entity)
        {
            MoveSystem.ChangeSpeed(entity, 10);
            MoveSystem.ChangeStopSpeed(entity, 5);
            FireSystem.ChangeSpeed(entity, 5);

            if (entity.GetComponent<AIComponent>() is { } component)
            {
                component.Enable = true;
            }
        }

        public void OnHealthChange(Actor entity, HealthComponent component)
        {
        }

        public static Actor Create(EcsEntity gameWorld, long actorId)
        {
            var actor = gameWorld.AddChild<Actor>(actorId, beforeAwake: x => x.Type = 1);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<HealthComponent>();
            actor.AddComponent<FireComponent>();
            actor.AddComponent<TaskListComponent>();
            actor.AddComponent<AIComponent>();
            return actor;
        }

    }
}
