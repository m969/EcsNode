using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class FireSystem : AComponentSystem<Actor, FireComponent>,
IAwake<Actor, FireComponent>
    {
        public void Awake(Actor entity, FireComponent component)
        {
        }

        public static void Shoot(TrueGame game, Actor actor, TSVector target)
        {
            var item = game.AddChild<Item>();
            item.AddComponent<TrueTransformComponent>();
            item.AddComponent<MoveComponent>();
            TrueTransformSystem.ChangePosition(item, actor.GetComponent<TrueTransformComponent>().Position);
            MoveSystem.SetSpeed(item, 30);
            MoveSystem.ChangeMove(item, target.normalized);

            EventSystem.Dispatch(actor.EcsNode, new EntityCreateCmd()
            {
                Entity = item
            });
        }

        public static void SetSpeed(Actor actor, int speed)
        {
            var moveComp = actor.GetComponent<FireComponent>();
            moveComp.FireSpeed = speed;
        }

        public static void ChangeFire(Actor actor, TSVector target)
        {
            var component = actor.GetComponent<FireComponent>();
            component.TrueDirection = target;
            component.FireState = true;
        }

        public static void StopFire(Actor actor)
        {
            var component = actor.GetComponent<FireComponent>();
            component.TrueDirection = TSVector.zero;
            component.FireState = false;
        }
    }
}
