using ECS;
using UnityEngine;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑效果组件，记录建筑对资源、人口、防御等的加成效果。
    /// 只包含属性数据，不包含方法逻辑。
    /// </summary>
    public class BuildingEffectComponent : EcsComponent
    {
        /// <summary>
        /// 资源产出加成
        /// </summary>
        public int ResourceBonus { get; set; }

        /// <summary>
        /// 人口上限加成
        /// </summary>
        public int PopulationBonus { get; set; }

        /// <summary>
        /// 防御加成
        /// </summary>
        public int DefenseBonus { get; set; }

        /// <summary>
        /// 是否激活建筑效果
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 当前资源产出
        /// </summary>
        public int ResourceOutput { get; set; }

        /// <summary>
        /// 当前人口上限
        /// </summary>
        public int PopulationLimit { get; set; }
    }
}
