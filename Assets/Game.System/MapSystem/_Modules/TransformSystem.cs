using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class TransformSystem : AComponentSystem<EcsEntity, TransformComponent>,
    IAwake<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {
        }

        public static void ChangePosition(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TransformComponent>().Position = target;

            EventSystem.Dispatch(actor.EcsNode, new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }

        public static void ChangeForward(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TransformComponent>().Forward = target;

            EventSystem.Dispatch(actor.EcsNode, new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }
    }
}