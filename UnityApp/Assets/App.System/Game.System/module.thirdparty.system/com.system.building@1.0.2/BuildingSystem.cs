using ECS;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑系统：通用业务入口与创建。
    /// </summary>
    public class BuildingSystem : AEntitySystem<BuildingEntity>,
        IAwake<BuildingEntity>, IInit<BuildingEntity>, IAfterInit<BuildingEntity>,
        IEnable<BuildingEntity>, IDisable<BuildingEntity>, IDestroy<BuildingEntity>
    {
        // 生命周期由框架调用，这里不放业务数据

        public void Awake(BuildingEntity entity) { }
        public void Init(BuildingEntity entity) { }
        public void AfterInit(BuildingEntity entity) { }
        public void Enable(BuildingEntity entity) { }
        public void Disable(BuildingEntity entity) { }
        public void Destroy(BuildingEntity entity) { }

        /// <summary>
        /// 创建建筑实体并挂载必要组件
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="type">建筑类型</param>
        /// <param name="position">地图坐标</param>
    /// <param name="ownerId">所属玩家ID</param>
    /// <param name="level">初始等级</param>
    public static BuildingEntity Create(EcsEntity parent, int type, Vector2Int position, long ownerId, int level = 1)
        {
            var entity = parent.AddChild<BuildingEntity>(e =>
            {
                e.Type = type;
                e.Position = position;
                e.OwnerId = ownerId;
                e.Level = level;
                e.State = BuildingState.Completed; // 默认处于已完成，具体建造由ConstructionSystem启动
            });
            entity.AddComponent<BuildingStateComponent>(c => { c.State = BuildingState.Completed; c.TimeLeft = 0f; });
            return entity;
        }

        /// <summary>
        /// 通用业务逻辑入口，可扩展具体功能
        /// </summary>
        /// <param name="entity">建筑实体</param>
        public static void HandleBuildingLogic(BuildingEntity entity)
        {
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state.State == BuildingState.Completed)
            {
                // 这里保留为占位扩展点
            }
        }
    }
}

