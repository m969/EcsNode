using ECS;
using UnityEngine;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑状态组件，记录建造、升级、拆除等状态及剩余时间。
    /// 只包含属性数据，不包含方法逻辑。
    /// </summary>
    public class BuildingStateComponent : EcsComponent
    {
        /// <summary>
        /// 当前状态（建造中/升级中/已完成/待拆除等）
        /// </summary>
        public BuildingState State { get; set; }

        /// <summary>
        /// 是否激活建造流程
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否已完成建造/升级
        /// </summary>
        public bool IsConstructed { get; set; }

        /// <summary>
        /// 当前建造/升级进度（秒）
        /// </summary>
        public float ConstructionProgress { get; set; }

        /// <summary>
        /// 当前建造/升级总时长（秒）
        /// </summary>
        public float ConstructionTime { get; set; }

        /// <summary>
        /// 剩余时间（秒）
        /// </summary>
        public float TimeLeft { get; set; }
    }
}
