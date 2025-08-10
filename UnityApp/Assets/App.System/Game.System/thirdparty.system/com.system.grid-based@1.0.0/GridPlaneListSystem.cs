using ECS;
using System;
using System.Collections.Generic;
using ECSGame.Module.GridBased;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域列表组件系统，负责网格区域实体列表的生命周期管理。
    /// </summary>
    public class GridPlaneListSystem : AComponentSystem<GridPlane, GridPlaneListComponent>, IAwake<GridPlane, GridPlaneListComponent>, IInit<GridPlane, GridPlaneListComponent>, IAfterInit<GridPlane, GridPlaneListComponent>, IEnable<GridPlane, GridPlaneListComponent>, IDisable<GridPlane, GridPlaneListComponent>
    {
        public void Awake(GridPlane entity, GridPlaneListComponent component) { }
        public void Init(GridPlane entity, GridPlaneListComponent component) { }
        public void AfterInit(GridPlane entity, GridPlaneListComponent component) { }
        public void Enable(GridPlane entity, GridPlaneListComponent component) { }
        public void Disable(GridPlane entity, GridPlaneListComponent component) { }
        /// <summary>
        /// 注册网格区域
        /// </summary>
        public static void RegisterGridPlane(GridPlane entity)
        {
            var component = entity.Parent.GetComponent<GridPlaneListComponent>();
            if (component.GridPlanes == null)
                component.GridPlanes = new Dictionary<long, GridPlane>();
            component.GridPlanes[entity.Id] = entity;
        }

        /// <summary>
        /// 注销网格区域
        /// </summary>
        public static void UnregisterGridPlane(GridPlane entity, GridPlaneListComponent component)
        {
            component.GridPlanes?.Remove(entity.Id);
        }

        /// <summary>
        /// 获取网格区域
        /// </summary>
        public static GridPlane GetGridPlane(long id, GridPlaneListComponent component)
        {
            if (component.GridPlanes != null && component.GridPlanes.TryGetValue(id, out var plane))
                return plane;
            return null;
        }
    }
}