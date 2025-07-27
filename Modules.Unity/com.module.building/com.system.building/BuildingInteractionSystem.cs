using ECS;
using UnityEngine;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑交互系统，处理玩家与建筑的交互，如点击、移动、放置等。
    /// 生命周期接口为成员函数，其余接口为静态方法。
    /// </summary>
    public class BuildingInteractionSystem : AEntitySystem<BuildingEntity>
    {
        public override void Awake(BuildingEntity entity)
        {
            // 初始化建筑实体状态
            // 可在此注册事件监听或初始化属性
        }

        public override void Enable(BuildingEntity entity)
        {
            // 激活建筑实体
            // 可用于显示建筑或允许交互
        }

        public override void Disable(BuildingEntity entity)
        {
            // 禁用建筑实体
            // 可用于隐藏建筑或禁止交互
        }

        public override void Destroy(BuildingEntity entity)
        {
            // 销毁建筑实体
            // 可在此清理资源或分发销毁事件
        }

        /// <summary>
        /// 玩家点击建筑时的业务逻辑处理。
        /// </summary>
        public static void OnClick(BuildingEntity entity)
        {
            // 处理玩家点击建筑的逻辑
            // 例如高亮建筑、弹出信息面板等
            entity.Dispatch<IOnBuildingClicked>(system => system.OnBuildingClicked(entity));
        }

        /// <summary>
        /// 玩家移动建筑时的业务逻辑处理。
        /// </summary>
        public static void OnMove(BuildingEntity entity, Vector2Int newPosition)
        {
            // 处理建筑移动逻辑
            // 更新建筑位置属性
            entity.Position = newPosition;
            entity.Dispatch<IOnBuildingMoved>(system => system.OnBuildingMoved(entity, newPosition));
        }

        /// <summary>
        /// 玩家放置建筑时的业务逻辑处理。
        /// </summary>
        public static void OnPlace(BuildingEntity entity, Vector2Int position)
        {
            // 处理建筑放置逻辑
            // 设置建筑最终位置并分发放置事件
            entity.Position = position;
            entity.Dispatch<IOnBuildingPlaced>(system => system.OnBuildingPlaced(entity, position));
        }
    }
}
