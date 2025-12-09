using ECS;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 单个格子实体，继承自EcsEntity，包含格子坐标、状态、占用者Id等属性。
    /// </summary>
    public class GridCell : EcsEntity
    {
        /// <summary>所属网格区域ID</summary>
        public long GridPlaneId { get; set; }
        /// <summary>格子X坐标（网格坐标系）</summary>
        public int X { get; set; }
        /// <summary>格子Y坐标（网格坐标系）</summary>
        public int Y { get; set; }
        /// <summary>格子状态（空置/占用）</summary>
        public GridCellState State { get; set; }
        /// <summary>占用者实体ID</summary>
        public long OccupiedById { get; set; }
    }
}
