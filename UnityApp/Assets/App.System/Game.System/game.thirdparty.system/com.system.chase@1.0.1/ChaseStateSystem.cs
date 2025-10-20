using ECS;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>维护追踪运行态的系统。</summary>
    public partial class ChaseStateSystem : AComponentSystem<EcsEntity, ChaseStateComponent>
    {
        /// <summary>读取当前追踪状态。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>追踪状态。</returns>
        public static ChaseState GetState(EcsEntity entity)
        {
            return entity.GetComponent<ChaseStateComponent>().State;
        }

        /// <summary>更新追踪状态。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="state">目标状态。</param>
        public static void SetState(EcsEntity entity, ChaseState state)
        {
            entity.GetComponent<ChaseStateComponent>().State = state;
        }

        /// <summary>读取当前追踪距离。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>与目标的距离。</returns>
        public static float GetCurrentDistance(EcsEntity entity)
        {
            return entity.GetComponent<ChaseStateComponent>().CurrentDistance;
        }

        /// <summary>更新追踪距离。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="distance">新的距离值。</param>
        public static void SetCurrentDistance(EcsEntity entity, float distance)
        {
            entity.GetComponent<ChaseStateComponent>().CurrentDistance = distance;
        }

        /// <summary>设置当前运动学数据。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="kinematics">运动学数据。</param>
        public static void SetKinematics(EcsEntity entity, in ChaseKinematics kinematics)
        {
            entity.GetComponent<ChaseStateComponent>().Kinematics = kinematics;
        }

        /// <summary>获取当前运动学数据。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>运动学数据。</returns>
        public static ChaseKinematics GetKinematics(EcsEntity entity)
        {
            return entity.GetComponent<ChaseStateComponent>().Kinematics;
        }
    }
}
