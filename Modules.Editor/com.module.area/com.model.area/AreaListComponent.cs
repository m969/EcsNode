using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    /// <summary>
    /// 区域列表组件
    /// </summary>
    public class AreaListComponent : EcsComponent
    {
        public List<Area> AreaList = new List<Area>();
        public Dictionary<long, Area> AreaDict = new Dictionary<long, Area>();
    }
}
