using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 提供用于访问和修改实体 <see cref="TransformComponent"/> 状态的辅助方法。
    /// </summary>
    public class TransformSystem : AComponentSystem<EcsEntity, TransformComponent>
    {
        /// <summary>
        /// 获取实体变换组件中的世界坐标。
        /// </summary>
        /// <param name="entity">需要查询的实体。</param>
        /// <returns>实体的世界坐标。</returns>
        public static TSVector GetPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Position;
        }

        /// <summary>
        /// 获取实体变换组件中的旋转信息。
        /// </summary>
        /// <param name="entity">需要查询的实体。</param>
        /// <returns>实体的旋转四元数。</returns>
        public static TSQuaternion GetRotation(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Rotation;
        }

        /// <summary>
        /// 获取实体变换组件中的朝向向量。
        /// </summary>
        /// <param name="entity">需要查询的实体。</param>
        /// <returns>朝向单位向量。</returns>
        public static TSVector GetForward(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Forward;
        }

        /// <summary>
        /// 获取实体变换组件中的预测位置。
        /// </summary>
        /// <param name="entity">需要查询的实体。</param>
        /// <returns>预测的世界坐标。</returns>
        public static TSVector GetForecastPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().ForecastPosition;
        }

        /// <summary>
        /// 设置实体变换组件中的世界坐标。
        /// </summary>
        /// <param name="entity">需要更新的实体。</param>
        /// <param name="position">新的世界坐标值。</param>
        public static void ChangePosition(EcsEntity entity, TSVector position)
        {
            entity.GetComponent<TransformComponent>().Position = position;
        }

        /// <summary>
        /// 设置实体变换组件中的旋转信息。
        /// </summary>
        /// <param name="entity">需要更新的实体。</param>
        /// <param name="rotation">新的旋转四元数。</param>
        public static void ChangeRotation(EcsEntity entity, TSQuaternion rotation)
        {
            entity.GetComponent<TransformComponent>().Rotation = rotation;
        }

        /// <summary>
        /// 设置实体变换组件中的预测位置。
        /// </summary>
        /// <param name="entity">需要更新的实体。</param>
        /// <param name="target">新的预测世界坐标。</param>
        public static void ChangeForecastPosition(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().ForecastPosition = target;
        }

        /// <summary>
        /// 设置实体变换组件中的朝向向量。
        /// </summary>
        /// <param name="entity">需要更新的实体。</param>
        /// <param name="target">新的朝向向量。</param>
        public static void ChangeForward(EcsEntity entity, TSVector target)
        {
            entity.GetComponent<TransformComponent>().Forward = target;
        }
    }
}