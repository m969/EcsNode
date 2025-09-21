using ECS;
using System;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 基于 DispatchStateComponent 的运行时 Tick 与规则校验系统。
    /// </summary>
    public class DispatchStateSystem : AComponentSystem<EcsEntity, DispatchStateComponent>,
        IAwake<EcsEntity, DispatchStateComponent>, IInit<EcsEntity, DispatchStateComponent>, IAfterInit<EcsEntity, DispatchStateComponent>,
        IEnable<EcsEntity, DispatchStateComponent>, IDisable<EcsEntity, DispatchStateComponent>, IDestroy<EcsEntity, DispatchStateComponent>
    {
        #region 生命周期 (空实现)
        public void Awake(EcsEntity entity, DispatchStateComponent c) { }
        public void Init(EcsEntity entity, DispatchStateComponent c) { }
        public void AfterInit(EcsEntity entity, DispatchStateComponent c) { }
        public void Enable(EcsEntity entity, DispatchStateComponent c) { }
        public void Disable(EcsEntity entity, DispatchStateComponent c) { }
        public void Destroy(EcsEntity entity, DispatchStateComponent c) { }
        #endregion

        /// <summary>
        /// 每帧调用以处理超时逻辑。
        /// 说明：组件系统内仅读取自身组件，无跨组件依赖。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="dt">增量时间（秒）。</param>
        public static void Tick(EcsEntity entity, float dt)
        {
            var state = entity.GetComponent<DispatchStateComponent>();
            if (state.State != DispatchState.Dispatching)
            {
                return;
            }
            if (state.RemainingTimeout > 0)
            {
                state.RemainingTimeout -= dt;
                if (state.RemainingTimeout <= 0)
                {
                    state.State = DispatchState.Timeout;
                    entity.Dispatch<IOnDispatchTimeout>(d => d.OnDispatchTimeout(entity));
                }
            }
        }

        /// <summary>
        /// 检查是否允许开始新派遣。
        /// 说明：按项目规范，避免在组件系统中跨组件读取，这里仅做基础参数校验。
        /// </summary>
        /// <param name="entity">实体（未使用）。</param>
        /// <param name="count">新派遣数量。</param>
        /// <returns>是否允许。</returns>
        public static bool CanDispatch(EcsEntity entity, int count)
        {
            return count > 0;
        }
    }
}
