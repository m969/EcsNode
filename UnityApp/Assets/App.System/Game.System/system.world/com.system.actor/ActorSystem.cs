using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using ECSGame.TaskModule;
using ECSGame.AttackModule;
using ECSGame.ActorStateModule;
using ET;
using ECSGame.ResourceDataModule;

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
        IDestroy<Actor>,
        IHealthChangeHandler,
        IStateEnterHandler
    {
        public static Actor Create(EcsEntity gameWorld, long actorId)
        {
            var actor = gameWorld.AddChild<Actor>(actorId, beforeAwake: x => x.Type = ActorType.Hero);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<FireComponent>();
            actor.AddComponent<ResourceDataComponent>();
            actor.AddComponent<TaskListComponent>();
            actor.AddComponent<AIComponent>();
            var healthComp = actor.AddComponent<HealthComponent>(beforeAwake: (comp) =>
            {
                comp.Health = 100;
                comp.MaxHealth = 100;
            });
            actor.AddComponent<ActorStateComponent>();

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

        public void Destroy(Actor actor)
        {
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

        public void OnHealthChangeHandle(Actor entity, HealthComponent component)
        {
            if (component.Health <= 0)
            {
                // 设置状态为死亡
                ActorStateSystem.RequestChangeState(entity, new ChangeStateRequest
                {
                    StateType = ActorStateType.Death,
                    Enable = true,
                    Force = true
                });
            }
        }

        public void OnStateEnterHandle(EcsEntity entity, ActorStateType stateType)
        {
            if (stateType == ActorStateType.Death)
            {
                // 处理角色死亡逻辑
                DisposeAfterSeconds(entity as Actor, 3).Coroutine();
                EventBus.Send(new ActorDeathEvent
                {
                    Actor = entity as Actor
                });
            }
        }

        public static async ETTask DisposeAfterSeconds(Actor actor, float seconds)
        {
            await TimerSystem.WaitAsync((int)(seconds * 1000));
            EcsObject.Destroy(actor);
            ActorListSystem.RemoveActor(actor.Parent, actor);
        }
    }
}
