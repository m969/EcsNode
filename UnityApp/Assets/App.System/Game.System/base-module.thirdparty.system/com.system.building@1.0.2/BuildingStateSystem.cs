using ECS;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑状态系统：推进状态与完成回调。
    /// </summary>
    public partial class BuildingStateSystem : AComponentSystem<BuildingEntity, BuildingStateComponent>
    {
        /// <summary>
        /// 定时推进状态（建造/升级进度等）
        /// </summary>
        /// <param name="entity">建筑实体</param>
        /// <param name="deltaTime">时间增量（秒）</param>
        public static void UpdateState(BuildingEntity entity, float deltaTime)
        {
            var component = entity.GetComponent<BuildingStateComponent>();
            // 派发状态心跳事件
            entity.Dispatch<IOnBuildingStateTick>(s => s.OnBuildingStateTick(entity, deltaTime));
            if (component.State == BuildingState.Constructing || component.State == BuildingState.Upgrading)
            {
                component.TimeLeft -= deltaTime;
                if (component.TimeLeft <= 0f)
                {
                    component.TimeLeft = 0f;
                    OnBuildComplete(entity);
                }
            }
        }

        /// <summary>
        /// 建造/升级完成回调
        /// </summary>
        /// <param name="entity">建筑实体</param>
        public static void OnBuildComplete(BuildingEntity entity)
        {
            var component = entity.GetComponent<BuildingStateComponent>();
            component.State = BuildingState.Completed;
            entity.State = BuildingState.Completed;
            // 派发完成事件
            entity.Dispatch<IOnBuildComplete>(s => s.OnBuildComplete(entity));
        }
    }
}
