using ECS;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域列表组件，继承自EcsComponent，管理网格区域实体列表。
    /// </summary>
    public class GridPlaneListComponent : EcsComponent
    {
        /// <summary>网格区域实体字典，key为网格区域ID</summary>
        public Dictionary<long, GridPlane> GridPlanes { get; set; }
        public Dictionary<int, GridPlane> ConfigId2GridPlanes { get; set; }
    }
}
