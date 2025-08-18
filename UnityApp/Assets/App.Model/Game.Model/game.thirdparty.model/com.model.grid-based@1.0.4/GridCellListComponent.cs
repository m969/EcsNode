using ECS;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 格子实体列表组件，继承自EcsComponent，管理格子实体列表。
    /// </summary>
    public class GridCellListComponent : EcsComponent
    {
        /// <summary>格子实体字典，key为格子坐标元组</summary>
        public Dictionary<(int x, int y), GridCell> Cells { get; set; } = new Dictionary<(int x, int y), GridCell>();
    }
}
