using ECS;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑建造系统：处理建造、升级、取消与拆除。
    /// </summary>
    public class BuildingConstructionSystem : AEntitySystem<BuildingEntity>,
        IAwake<BuildingEntity>, IInit<BuildingEntity>, IAfterInit<BuildingEntity>,
        IEnable<BuildingEntity>, IDisable<BuildingEntity>, IDestroy<BuildingEntity>
    {
    public void Awake(BuildingEntity entity) { }
    public void Init(BuildingEntity entity) { }
    public void AfterInit(BuildingEntity entity) { }
    public void Enable(BuildingEntity entity) { }
    public void Disable(BuildingEntity entity) { }
    public void Destroy(BuildingEntity entity) { }
        /// <summary>
        /// 发起建造，targetLevel为目标等级
        /// </summary>
        /// <param name="entity">建筑实体</param>
        /// <param name="targetLevel">目标等级</param>
        public static void StartBuild(BuildingEntity entity, int targetLevel)
        {
            var state = entity.GetComponent<BuildingStateComponent>();
            if (entity.Level < 1) entity.Level = 1;
            // 同步实体与组件状态
            state.State = BuildingState.Constructing;
            entity.State = BuildingState.Constructing;
            // 派发开始建造事件
            entity.Dispatch<IOnStartBuild>(s => s.OnStartBuild(entity, targetLevel));
        }

        /// <summary>
        /// 发起升级
        /// </summary>
        /// <param name="entity">建筑实体</param>
        /// <param name="targetLevel">目标等级</param>
        public static void UpgradeBuild(BuildingEntity entity, int targetLevel)
        {
            var state = entity.GetComponent<BuildingStateComponent>();
            state.State = BuildingState.Upgrading;
            entity.State = BuildingState.Upgrading;
            // 派发开始升级事件
            entity.Dispatch<IOnUpgradeBuild>(s => s.OnUpgradeBuild(entity, targetLevel));
        }

        /// <summary>
        /// 取消建造/升级
        /// </summary>
        /// <param name="entity">建筑实体</param>
        public static void CancelBuild(BuildingEntity entity)
        {
            var state = entity.GetComponent<BuildingStateComponent>();
            state.TimeLeft = 0f;
            // 回退为完成态，等待下一次操作
            state.State = BuildingState.Completed;
            entity.State = BuildingState.Completed;
            // 派发取消建造事件
            entity.Dispatch<IOnCancelBuild>(s => s.OnCancelBuild(entity));
        }

        /// <summary>
        /// 拆除建筑
        /// </summary>
        /// <param name="entity">建筑实体</param>
        public static void RemoveBuild(BuildingEntity entity)
        {
            var state = entity.GetComponent<BuildingStateComponent>();
            state.State = BuildingState.ToBeRemoved;
            entity.State = BuildingState.ToBeRemoved;
            state.TimeLeft = 0f;
            // 派发拆除事件
            entity.Dispatch<IOnRemoveBuild>(s => s.OnRemoveBuild(entity));
        }
    }
}

