using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成条件组件（列表）
    /// </summary>
    public class AchieveConditionListComponent : EcsComponent
    {
        /// <summary>条件集合</summary>
        public List<AchieveCondition> ConditionList { get; set; } = new List<AchieveCondition>();
        /// <summary>是否全部满足</summary>
        public bool IsAllSatisfied { get; set; }
        /// <summary>当前进度</summary>
        public int CurrentProgress { get; set; }
        /// <summary>总进度</summary>
        public int TotalProgress { get; set; }
        /// <summary>进度百分比</summary>
        public float ProgressPercentage { get; set; }
    }
}
