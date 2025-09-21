using ECS;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// UnitDispatcher 实体的生命周期系统与业务静态方法。
    /// </summary>
    public partial class UnitDispatcherSystem : AEntitySystem<UnitDispatcher>,
        IAwake<UnitDispatcher>, IInit<UnitDispatcher>, IAfterInit<UnitDispatcher>,
        IEnable<UnitDispatcher>, IDisable<UnitDispatcher>, IDestroy<UnitDispatcher>
    {
        #region 生命周期 (空实现 - 业务在静态方法中)
        public void Awake(UnitDispatcher entity) { }
        public void Init(UnitDispatcher entity) { }
        public void AfterInit(UnitDispatcher entity) { }
        public void Enable(UnitDispatcher entity) { }
        public void Disable(UnitDispatcher entity) { }
        public void Destroy(UnitDispatcher entity) { }
        #endregion

        /// <summary>
        /// 创建并附加一个 UnitDispatcher 实体作为宿主实体的子实体。
        /// </summary>
        /// <param name="hostEntity">宿主实体。</param>
        /// <param name="cfgId">配置 Id。</param>
        /// <returns>新创建的 UnitDispatcher 实例。</returns>
        public static UnitDispatcher Create(EcsEntity hostEntity, int cfgId)
        {
            return hostEntity.AddChild<UnitDispatcher>(exec =>
            {
                exec.ConfigId = cfgId;
                exec.DispatchCount = 0;
                exec.TargetEntityId = 0;
                exec.Timeout = 0f;
            });
        }

        /// <summary>
        /// 尝试开始一次派遣：基于执行实体更新其宿主的 DispatchStateComponent。
        /// 注意：目标与超时从 exec.TargetEntityId 与 exec.Timeout 读取。
        /// </summary>
        /// <param name="exec">执行实体。</param>
        /// <param name="count">派遣数量（累加）。</param>
        /// <returns>是否成功开始派遣。</returns>
        public static bool StartDispatch(UnitDispatcher exec, int count)
        {
            var state = exec.GetComponent<DispatchStateComponent>();
            if (state.State == DispatchState.Dispatching)
            {
                return false; // 并发保护
            }

            if (!DispatchStateSystem.CanDispatch(exec, count))
            {
                return false;
            }

            exec.DispatchCount = count;

            state.State = DispatchState.Dispatching;
            state.TargetEntityId = exec.TargetEntityId;
            state.RemainingTimeout = exec.Timeout;

            exec.Dispatch<IOnDispatchStarted>(d => d.OnDispatchStarted(exec, count));
            return true;
        }

        /// <summary>
        /// 取消当前派遣（基于执行实体的宿主状态组件）。
        /// </summary>
        public static void CancelDispatch(UnitDispatcher exec)
        {
            var state = exec.GetComponent<DispatchStateComponent>();
            if (state.State != DispatchState.Dispatching)
            {
                return; // 非派遣中无需处理
            }

            state.State = DispatchState.Cancelled;
            state.TargetEntityId = 0;
            state.RemainingTimeout = 0;

            exec.Dispatch<IOnDispatchCancelled>(d => d.OnDispatchCancelled(exec));
        }

        /// <summary>
        /// 标记派遣完成并派发完成事件（基于执行实体的宿主）。
        /// </summary>
        public static void CompleteDispatch(UnitDispatcher exec)
        {
            var state = exec.GetComponent<DispatchStateComponent>();
            if (state.State != DispatchState.Dispatching)
            {
                return;
            }

            state.State = DispatchState.Completed;
            state.TargetEntityId = 0;
            state.RemainingTimeout = 0;

            exec.Dispatch<IOnDispatchCompleted>(d => d.OnDispatchCompleted(exec));
        }
    }
}
