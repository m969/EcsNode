using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorFireOnceSystem : AEntitySystem<Actor>, IFireOnceHandler
    {
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
            EcsObject.Init(item);
        }
    }
}
