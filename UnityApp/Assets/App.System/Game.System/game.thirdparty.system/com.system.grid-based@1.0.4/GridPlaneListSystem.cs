using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域列表组件系统，管理World上的GridPlane列表。
    /// </summary>
    public class GridPlaneListSystem : AComponentSystem<EcsEntity, GridPlaneListComponent>, 
        IAwake<EcsEntity, GridPlaneListComponent>, 
        IInit<EcsEntity, GridPlaneListComponent>, 
        IAfterInit<EcsEntity, GridPlaneListComponent>, 
        IEnable<EcsEntity, GridPlaneListComponent>, 
        IDisable<EcsEntity, GridPlaneListComponent>, 
        IDestroy<EcsEntity, GridPlaneListComponent>
    {
        public void Awake(EcsEntity entity, GridPlaneListComponent component) { }
        public void Init(EcsEntity entity, GridPlaneListComponent component) { }
        public void AfterInit(EcsEntity entity, GridPlaneListComponent component) { }
        public void Enable(EcsEntity entity, GridPlaneListComponent component) { }
        public void Disable(EcsEntity entity, GridPlaneListComponent component) { }
        public void Destroy(EcsEntity entity, GridPlaneListComponent component) { }

        /// <summary>添加网格区域</summary>
        public static void AddGridPlane(EcsEntity entity, GridPlane gridPlane)
        {
            var comp = entity.GetComponent<GridPlaneListComponent>();
            comp.GridPlanes ??= new Dictionary<long, GridPlane>();
            comp.GridPlanes[gridPlane.Id] = gridPlane;
        }

        /// <summary>移除网格区域</summary>
        public static void RemoveGridPlane(EcsEntity entity, long gridPlaneId)
        {
            var comp = entity.GetComponent<GridPlaneListComponent>();
            comp.GridPlanes.Remove(gridPlaneId);
        }

        /// <summary>根据Id获取网格区域</summary>
        public static GridPlane GetGridPlane(EcsEntity entity, long id)
        {
            var comp = entity.GetComponent<GridPlaneListComponent>();
            return comp.GridPlanes[id];
        }
    }
}