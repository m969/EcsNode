using ECS;
using System;
using System.Collections.Generic;
using ECSGame.Module.GridBased;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 格子实体列表组件系统，负责格子实体列表的生命周期管理。
    /// </summary>
    public class GridCellListSystem : AComponentSystem<GridPlane, GridCellListComponent>, IAwake<GridPlane, GridCellListComponent>, IInit<GridPlane, GridCellListComponent>, IAfterInit<GridPlane, GridCellListComponent>, IEnable<GridPlane, GridCellListComponent>, IDisable<GridPlane, GridCellListComponent>, IDestroy<GridPlane, GridCellListComponent>
    {
        public void Awake(GridPlane entity, GridCellListComponent component) { }
        public void Init(GridPlane entity, GridCellListComponent component) { }
        public void AfterInit(GridPlane entity, GridCellListComponent component) { }
        public void Enable(GridPlane entity, GridCellListComponent component) { }
        public void Disable(GridPlane entity, GridCellListComponent component) { }
        public void Destroy(GridPlane entity, GridCellListComponent component) { }

        /// <summary>获取指定坐标的格子</summary>
        public static GridCell GetCell(GridPlane entity, int x, int y)
        {
            var comp = entity.GetComponent<GridCellListComponent>();
            return comp.Cells[(x, y)];
        }

        /// <summary>获取所有空置格子</summary>
        public static IEnumerable<GridCell> GetEmptyCells(GridPlane entity)
        {
            var comp = entity.GetComponent<GridCellListComponent>();
            foreach (var cell in comp.Cells.Values)
            {
                if (cell.State == GridCellState.Empty)
                    yield return cell;
            }
        }
    }
}
