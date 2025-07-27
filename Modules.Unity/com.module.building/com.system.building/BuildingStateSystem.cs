using ECS;
using UnityEngine;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑状态系统，定时更新建筑状态，处理建造完成、升级完成等事件。
    /// 生命周期接口为成员函数，其余接口为静态方法。
    /// </summary>
    public class BuildingStateSystem : AComponentSystem<BuildingEntity, BuildingStateComponent>
    {
        public override void Awake(BuildingEntity entity, BuildingStateComponent component)
        {
            // 初始化建筑状态，设置为未建造或初始状态
            component.State = BuildingState.NotBuilt;
            component.Progress = 0f;
        }

        public override void Init(BuildingEntity entity, BuildingStateComponent component)
        {
            // 初始化建造进度和状态
            component.Progress = 0f;
            component.State = BuildingState.Building;
        }

        public override void AfterInit(BuildingEntity entity, BuildingStateComponent component)
        {
            // 可用于触发建造开始事件
            entity.Dispatch<IOnStartBuild>(system => system.OnStartBuild(entity, component.Progress));
        }

        public override void Enable(BuildingEntity entity, BuildingStateComponent component)
        {
            // 激活建筑状态，允许进度更新
            component.IsActive = true;
        }

        public override void Disable(BuildingEntity entity, BuildingStateComponent component)
        {
            // 禁用建筑状态，暂停进度更新
            component.IsActive = false;
        }

        public override void Destroy(BuildingEntity entity, BuildingStateComponent component)
        {
            // 清理建筑状态相关数据
            component.State = BuildingState.Destroyed;
            component.Progress = 0f;
            component.IsActive = false;
        }

        /// <summary>
        /// 建造完成回调，处理建筑状态变更及后续逻辑。
        /// </summary>
        public static void OnBuildComplete(BuildingEntity entity)
        {
            // 获取组件，处理建造完成逻辑
            var component = entity.GetComponent<BuildingStateComponent>();
            if (component != null)
            {
                component.State = BuildingState.Completed;
                component.Progress = 1f;
                // 分发建造完成事件
                entity.Dispatch<IOnBuildComplete>(system => system.OnBuildComplete(entity));
            }
        }
    }
}
