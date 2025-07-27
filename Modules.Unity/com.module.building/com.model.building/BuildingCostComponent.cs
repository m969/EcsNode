using ECS;
using UnityEngine;
using System.Collections.Generic;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑消耗组件，记录建造或升级所需资源类型及数量。
    /// 只包含属性数据，不包含方法逻辑。
    /// </summary>
    public class BuildingCostComponent : EcsComponent
    {
        /// <summary>
        /// 资源类型及数量（如{"Wood":100, "Stone":50}）
        /// </summary>
        public Dictionary<string, int> Cost { get; set; }

        /// <summary>
        /// 建造/升级所需时间（秒）
        /// </summary>
        public float BuildTime { get; set; }
    }
}
