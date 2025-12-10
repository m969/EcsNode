using ECS;
using System;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 派遣代理系统。
    /// 负责 DispatchAgentComponent 的状态流转与超时更新。
    /// </summary>
    public class DispatchAgentSystem : AComponentSystem<EcsEntity, DispatchAgentComponent>
    {
        /// <summary>
        /// 每帧调用以处理超时逻辑。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="dt">增量时间（秒）。</param>
        public static void Tick(EcsEntity entity, float dt)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent.State != DispatchState.Dispatching)
            {
                return;
            }
            if (agent.RemainingTimeout > 0)
            {
                agent.RemainingTimeout -= dt;
                if (agent.RemainingTimeout <= 0)
                {
                    agent.State = DispatchState.Timeout;
                    entity.Dispatch<IOnDispatchTimeout>(d => d.OnDispatchTimeout(entity));
                }
            }
        }

        /// <summary>
        /// 检查是否允许开始新派遣。
        /// </summary>
        public static bool CanDispatch(EcsEntity entity, int count)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent == null) return false;
            
            // 只有空闲状态才能开始新派遣
            if (agent.State != DispatchState.Idle) return false;

            return count > 0;
        }

        /// <summary>
        /// 尝试开始一次派遣。
        /// </summary>
        /// <param name="entity">宿主实体。</param>
        /// <param name="targetId">目标实体ID。</param>
        /// <param name="count">派遣数量。</param>
        /// <param name="timeout">超时时间。</param>
        /// <returns>是否成功开始派遣。</returns>
        public static bool StartDispatch(EcsEntity entity, long targetId, int count, float timeout)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent == null) return false;

            if (agent.State == DispatchState.Dispatching)
            {
                return false; // 并发保护
            }

            if (!CanDispatch(entity, count))
            {
                return false;
            }

            agent.CurrentDispatchCount = count;
            agent.State = DispatchState.Dispatching;
            agent.TargetEntityId = targetId;
            agent.RemainingTimeout = timeout;

            entity.Dispatch<IOnDispatchStarted>(d => d.OnDispatchStarted(entity, count));
            return true;
        }

        /// <summary>
        /// 取消当前派遣。
        /// </summary>
        public static void CancelDispatch(EcsEntity entity)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent == null || agent.State != DispatchState.Dispatching)
            {
                return; // 非派遣中无需处理
            }

            agent.State = DispatchState.Cancelled;
            agent.TargetEntityId = 0;
            agent.RemainingTimeout = 0;

            entity.Dispatch<IOnDispatchCancelled>(d => d.OnDispatchCancelled(entity));
        }

        /// <summary>
        /// 标记派遣完成并派发完成事件。
        /// </summary>
        public static void CompleteDispatch(EcsEntity entity)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent == null || agent.State != DispatchState.Dispatching)
            {
                return;
            }

            agent.State = DispatchState.Completed;
            agent.TargetEntityId = 0;
            agent.RemainingTimeout = 0;

            entity.Dispatch<IOnDispatchCompleted>(d => d.OnDispatchCompleted(entity));
        }
        
        /// <summary>
        /// 重置为 Idle 状态（通常在完成或取消后的清理阶段调用）。
        /// </summary>
        public static void ResetToIdle(EcsEntity entity)
        {
            var agent = entity.GetComponent<DispatchAgentComponent>();
            if (agent == null) return;
            
            agent.State = DispatchState.Idle;
            agent.TargetEntityId = 0;
            agent.RemainingTimeout = 0;
            agent.CurrentDispatchCount = 0;
        }
    }
}
