using ECS;
using System;
using System.Collections.Generic;
using ECSGame.Module.GridBased;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格状态变更事件接口
    /// </summary>
    public interface IOnGridCellStateChanged : IDispatch
    {
        void OnGridCellStateChanged(GridCell entity, int x, int y, GridCellState newState);
    }

    /// <summary>
    /// 格子实体系统，负责格子的创建和生命周期管理。
    /// </summary>
    public class GridCellSystem : AEntitySystem<GridCell>, IAwake<GridCell>, IInit<GridCell>, IAfterInit<GridCell>, IEnable<GridCell>, IDisable<GridCell>, IDestroy<GridCell>
    {
        /// <summary>
        /// 创建格子实体
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="x">格子X坐标</param>
        /// <param name="y">格子Y坐标</param>
        /// <returns>格子实体</returns>
        public static GridCell CreateGridCell(GridPlane parent, int x, int y)
        {
            var cell = parent.AddChild<GridCell>(e =>
            {
                e.GridPlaneId = parent.Id;
                e.X = x;
                e.Y = y;
                e.State = GridCellState.Empty;
            });
            return cell;
        }

        /// <summary>
        /// 更新格子状态
        /// </summary>
        /// <param name="entity">格子实体</param>
        /// <param name="state">新状态</param>
        /// <param name="occupierId">占用者ID</param>
        public static void UpdateCellState(GridCell entity, GridCellState state, long occupierId = 0)
        {
            entity.State = state;
            entity.OccupiedById = occupierId;
            // 派发格子状态变更事件
            entity.Dispatch<IOnGridCellStateChanged>(sys => sys.OnGridCellStateChanged(entity, entity.X, entity.Y, state));
        }

        public void Awake(GridCell entity) { }
        public void Init(GridCell entity) { }
        public void AfterInit(GridCell entity) { }
        public void Enable(GridCell entity) { }
        public void Disable(GridCell entity) { }
        public void Destroy(GridCell entity) { }
    }
}
