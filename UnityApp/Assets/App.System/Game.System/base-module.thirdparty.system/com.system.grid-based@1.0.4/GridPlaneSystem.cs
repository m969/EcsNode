using ECS;
using ECSGame.Module.GridBased;
using System;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域变更事件接口
    /// </summary>
    public interface IOnGridPlaneChanged : IDispatch
    {
        void OnGridPlaneChanged(GridPlane entity);
    }

    /// <summary>
    /// 网格区域实体系统，负责网格区域的创建和生命周期管理。
    /// </summary>
    public class GridPlaneSystem : AEntitySystem<GridPlane>, IAwake<GridPlane>, IInit<GridPlane>, IAfterInit<GridPlane>, IEnable<GridPlane>, IDisable<GridPlane>, IUpdate<GridPlane>, IDestroy<GridPlane>
    {
        /// <summary>
        /// 创建并初始化网格区域实体
        /// </summary>
        /// <param name="parent">父实体（World实体）</param>
        /// <param name="config">网格区域配置接口</param>
        /// <param name="position">网格区域世界坐标位置</param>
        /// <returns>网格区域实体</returns>
        public static GridPlane CreateGridPlane(EcsEntity parent, IGridPlaneConfig config, (float x, float y) position)
        {
            // 创建GridPlane实体并设置基础属性
            var gridPlane = parent.AddChild<GridPlane>(e =>
            {
                e.ConfigId = config.Id;
                e.Position = position;
            });
            // 添加格子列表组件和坐标转换组件
            gridPlane.AddComponent<GridCellListComponent>(null);
            gridPlane.AddComponent<GridConverterComponent>(c =>
            {
                c.GridPlaneId = gridPlane.Id;
                c.Position = position;
                c.CellSize = config.CellSize;
            });
            // 初始化网格尺寸和单元格大小，并派发变更事件
            InitializeGrid(gridPlane, config);
            // 创建格子实体并添加到GridCellListComponent字典
            for (int x = 0; x < config.Width; x++)
            {
                for (int y = 0; y < config.Height; y++)
                {
                    var cell = GridCellSystem.CreateGridCell(gridPlane, x, y);
                    GridCellListSystem.AddCell(gridPlane, cell);
                }
            }
            return gridPlane;
        }

        /// <summary>
        /// 初始化网格区域
        /// </summary>
        /// <param name="entity">网格区域实体</param>
        /// <param name="config">网格区域配置</param>
        public static void InitializeGrid(GridPlane entity, IGridPlaneConfig config)
        {
            entity.Width = config.Width;
            entity.Height = config.Height;
            entity.CellSize = config.CellSize;
            // 派发网格区域变更事件
            entity.Dispatch<IOnGridPlaneChanged>(sys => sys.OnGridPlaneChanged(entity));
        }

        public void Awake(GridPlane entity) { }
        public void Init(GridPlane entity) { }
        public void AfterInit(GridPlane entity) { }
        public void Enable(GridPlane entity) { }
        public void Disable(GridPlane entity) { }
        public void Update(GridPlane entity) { }
        public void Destroy(GridPlane entity) { }
    }
}
