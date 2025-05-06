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

        public static TSVector GetPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Position;
        }

        public static TSVector GetForecastPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().ForecastPosition;
        }

        public static void ChangePosition(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().Position = target;
            EntitySystem.ComponentChange<EcsEntity, TransformComponent>(entity);
        }

        public static void ChangeForecastPosition(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().ForecastPosition = target;
            EntitySystem.ComponentChange<EcsEntity, TransformComponent>(entity);
        }

        public static void ChangeForward(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().Forward = target;
            EntitySystem.ComponentChange<EcsEntity, TransformComponent>(entity);
        }
    }
}