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

        public static TSQuaternion GetRotation(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Rotation;
        }

        public static TSVector GetForward(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Forward;
        }

        public static TSVector GetForecastPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().ForecastPosition;
        }

        public static void ChangePosition(EcsEntity entity, TSVector position)
        {
            entity.GetComponent<TransformComponent>().Position = position;
        }

        public static void ChangeRotation(EcsEntity entity, TSQuaternion rotation)
        {
            entity.GetComponent<TransformComponent>().Rotation = rotation;
        }

        public static void ChangeForecastPosition(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().ForecastPosition = target;
        }

        public static void ChangeForward(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().Forward = target;
        }
    }
}