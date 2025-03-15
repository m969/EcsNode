using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class TrueTransformSystem : AComponentSystem<Actor, TrueTransformComponent>,
    IAwake<Actor, TrueTransformComponent>
    {
        public void Awake(Actor entity, TrueTransformComponent component)
        {
        }

        public static void ChangeLook(Actor actor, TSVector target)
        {
            actor.GetComponent<TrueTransformComponent>().Forward = target;

            EventSystem.Dispatch(actor.EcsNode, new EntityViewUpdateCmd()
            {
                Entity = actor,
                Component = actor.GetComponent<TrueTransformComponent>(),
            });
        }
    }
}