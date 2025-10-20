using ECS;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>提供追踪运行时所需空间与速度数据的接口。</summary>
    public interface IChaseRuntimeProvider : IDispatch
    {
        /// <summary>获取追踪实体当前所在位置。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <returns>实体当前位置。</returns>
        Vector3 GetOwnerPosition(EcsEntity entity);

        /// <summary>获取指定目标的当前所在位置。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标实体 Id。</param>
        /// <returns>目标当前位置。</returns>
        Vector3 GetTargetPosition(EcsEntity entity, long targetId);

        /// <summary>获取追踪实体相对于目标的速度标量。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="targetId">目标实体 Id。</param>
        /// <returns>相对速度标量。</returns>
        float GetRelativeSpeed(EcsEntity entity, long targetId);
    }
}
