using ECS;
using System;
using System.Collections.Generic;
using ECSGame.Module.GridBased;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 格子坐标转换组件系统，负责格子坐标与世界坐标的转换功能生命周期管理。
    /// </summary>
    public class GridConverterSystem : AComponentSystem<GridPlane, GridConverterComponent>, IAwake<GridPlane, GridConverterComponent>, IInit<GridPlane, GridConverterComponent>, IAfterInit<GridPlane, GridConverterComponent>, IEnable<GridPlane, GridConverterComponent>, IDisable<GridPlane, GridConverterComponent>
    {
        public void Awake(GridPlane entity, GridConverterComponent component) { }
        public void Init(GridPlane entity, GridConverterComponent component) { }
        public void AfterInit(GridPlane entity, GridConverterComponent component) { }
        public void Enable(GridPlane entity, GridConverterComponent component) { }
        public void Disable(GridPlane entity, GridConverterComponent component) { }
        /// <summary>
        /// 网格坐标转世界坐标
        /// </summary>
        public static (float x, float y) GridToWorld(GridPlane entity, int gridX, int gridY)
        {
            var comp = entity.GetComponent<GridConverterComponent>();
            float wx = comp.Position.x + gridX * comp.CellSize;
            float wy = comp.Position.y + gridY * comp.CellSize;
            return (wx, wy);
        }

        /// <summary>
        /// 世界坐标转网格坐标
        /// </summary>
        public static (int x, int y) WorldToGrid(GridPlane entity, float worldX, float worldY)
        {
            var comp = entity.GetComponent<GridConverterComponent>();
            int gx = (int)((worldX - comp.Position.x) / comp.CellSize);
            int gy = (int)((worldY - comp.Position.y) / comp.CellSize);
            return (gx, gy);
        }

        /// <summary>
        /// 检查坐标是否在网格区域内
        /// </summary>
        public static bool IsInBounds(GridPlane entity, int x, int y)
        {
            return x >= 0 && x < entity.Width && y >= 0 && y < entity.Height;
        }
    }
}