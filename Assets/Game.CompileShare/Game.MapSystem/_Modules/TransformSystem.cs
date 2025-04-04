using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using static UnityEngine.GraphicsBuffer;

namespace ECSGame
{
    public class TransformSystem : AComponentSystem<EcsEntity, TransformComponent>,
    IAwake<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {
        }

        public static TSVector GetPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Position;
        }

        public static TSVector GetForecastPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().ForecastPosition;
        }

        public static void ChangePosition(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TransformComponent>().Position = target;

            EventSystem.Dispatch(new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }

        public static void ChangeForward(EcsEntity actor, TSVector target)
        {
            actor.GetComponent<TransformComponent>().Forward = target;

            EventSystem.Dispatch(new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }
    }
}