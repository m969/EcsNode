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
        IUpdate<Actor>,
        IHealthChangeHandler
    {
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

            // 追逐模块
            actor.AddComponent<ChaseModule.ChaseComponent>();
            actor.AddComponent<ChaseModule.ChaseStateComponent>();
            actor.AddComponent<ChaseModule.ChaseConfigComponent>();

            return actor;
        }

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

        public void Update(Actor entity)
        {
            if (entity.GetComponent<AIComponent>() is { } component)
            {
                AISystem.Update(entity, component, 0);
            }
            if (entity.GetComponent<MoveComponent>() is { } moveComp)
            {
                MoveSystem.Update(entity, moveComp, moveComp.TrueDirection);
            }
            if (entity.GetComponent<ChaseModule.ChaseComponent>() is { } chaseComp)
            {
                ChaseModule.ChaseSystem.Tick(entity, AppStatic.DeltaTimeSeconds);
            }
        }

        public void OnHealthChange(Actor entity, HealthComponent component)
        {
        }

        // public EcsEntity Resolve(EcsEntity entity, long targetId)
        // {
        //     var world = entity.GetParent<World>();
        //     return ActorListSystem.GetActor(world, targetId);
        // }
    }
}
