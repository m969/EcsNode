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
        /// 创建网格区域实体
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="configId">网格配置ID</param>
        /// <param name="position">网格区域位置</param>
        /// <returns>网格区域实体</returns>
        public static GridPlane CreateGridPlane(EcsEntity parent, int configId, (float x, float y) position)
        {
            var gridPlane = parent.AddChild<GridPlane>(e =>
            {
                e.ConfigId = configId;
                e.Position = position;
            });
            return gridPlane;
        }

        /// <summary>
        /// 初始化网格区域
        /// </summary>
        /// <param name="entity">网格区域实体</param>
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
