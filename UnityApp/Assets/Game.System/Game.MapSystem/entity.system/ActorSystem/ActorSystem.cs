using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IUpdate<Actor>,
        IHealthChangeHandler,
        IFireOnceHandler
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

        public void Update(Actor entity)
        {

        }

        public void OnFireOnce(Actor actor, TSVector target)
        {
            var game = actor.GetParent<TrueGame>();
            var item = game.AddChild<Item>();
            item.AddComponent<TransformComponent>();
            item.AddComponent<CollisionComponent>();
            item.AddComponent<MoveComponent>();
            item.GetComponent<CollisionComponent>().Layer = actor.GetComponent<CollisionComponent>().Layer;
            var actorTrans = actor.GetComponent<TransformComponent>();
            TransformSystem.ChangePosition(item, actorTrans.ForecastPosition);
            TransformSystem.ChangeForward(item, actorTrans.Forward);
            MoveSystem.ChangeSpeed(item, 150);
            MoveSystem.ChangeDirection(item, actorTrans.Forward.normalized);
            EventSystem.Init(item);
        }

        public void OnHealthChange(Actor entity, HealthComponent component)
        {
        }

        public static Actor Create(TrueGame game, long actorId)
        {
            var actor = game.AddChild<Actor>(actorId, beforeAwake: x => x.Type = 1);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<HealthComponent>();
            actor.AddComponent<FireComponent>();
            return actor;
        }

    }
}
