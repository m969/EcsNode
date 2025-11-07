using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public interface IFireOnceHandler : IDispatch
    {
        void OnFireOnce(Actor entity, TSVector target);
    }

    public class FireSystem : AComponentSystem<Actor, FireComponent>,
IAwake<Actor, FireComponent>
    {
        public void Awake(Actor entity, FireComponent component)
        {
        }

        public static void FireOnce(Actor actor, TSVector target)
        {
            actor.Dispatch<IFireOnceHandler>(anySystem => anySystem.OnFireOnce(actor, target));
        }

        public static void ChangeSpeed(Actor actor, int speed)
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
