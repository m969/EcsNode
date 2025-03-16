using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class TrueTransformSystem : AComponentSystem<EcsEntity, TrueTransformComponent>,
    IAwake<EcsEntity, TrueTransformComponent>
    {
        public void Awake(EcsEntity entity, TrueTransformComponent component)
        {
        }

        public static void ChangePosition(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TrueTransformComponent>().Position = target;

            EventSystem.Dispatch(actor.EcsNode, new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TrueTransformComponent>(),
            });
        }

        public static void ChangeRotation(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TrueTransformComponent>().Forward = target;

            EventSystem.Dispatch(actor.EcsNode, new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TrueTransformComponent>(),
            });
        }
    }
}