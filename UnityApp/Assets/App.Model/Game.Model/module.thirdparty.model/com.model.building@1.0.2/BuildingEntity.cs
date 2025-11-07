using ECS;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑实体，记录建筑类型、等级、位置、状态、所属玩家等信息。
    /// 只包含属性数据，不包含方法逻辑。
    /// </summary>
    public class BuildingEntity : EcsEntity
    {
        /// <summary>
        /// 建筑类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 建筑等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 地图坐标（x, y）
        /// </summary>
        public Vector2Int Position { get; set; }

        /// <summary>
        /// 建造/升级/完成/拆除等状态（枚举类型）
        /// </summary> 
        public BuildingState State { get; set; }

    /// <summary>
    /// 所属玩家ID
    /// </summary>
    public long OwnerId { get; set; }
    }

    /// <summary>
    /// 建筑状态枚举
    /// </summary>
    public enum BuildingState
    {
        /// <summary>
        /// 正在建造
        /// </summary>
        Constructing,
        /// <summary>
        /// 升级中
        /// </summary>
        Upgrading,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed,
        /// <summary>
        /// 已拆除
        /// </summary>
        ToBeRemoved
    }
}
