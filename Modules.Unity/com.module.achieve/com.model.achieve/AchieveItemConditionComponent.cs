using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成条件组件，管理多个达成条件实体（AchieveCondition）
    /// </summary>
    public class AchieveItemConditionComponent : EcsComponent
    {
        /// <summary>
        /// 达成条件实体集合
        /// </summary>
        public List<AchieveCondition> ConditionList { get; set; } = new List<AchieveCondition>();

        /// <summary>
        /// 条件逻辑类型（全部满足/任一满足/权重计算）
        /// </summary>
        public ConditionLogicType LogicType { get; set; }

        /// <summary>
        /// 是否全部条件满足
        /// </summary>
        public bool IsAllSatisfied { get; set; }

        /// <summary>
        /// 已满足条件数量
        /// </summary>
        public int SatisfiedCount { get; set; }

        /// <summary>
        /// 动态计算的总进度值（基于所有条件的目标值总和）
        /// </summary>
        public int TotalProgress { get; set; }

        /// <summary>
        /// 动态计算的当前进度值（基于所有条件的当前值总和）
        /// </summary>
        public int CurrentProgress { get; set; }

        /// <summary>
        /// 动态计算的进度百分比（CurrentProgress/TotalProgress）
        /// </summary>
        public float ProgressPercentage { get; set; }

        /// <summary>
        /// 里程碑节点（阶段性奖励点）
        /// </summary>
        public List<int> Milestones { get; set; } = new List<int>();

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// 同步状态（本地/已同步/待同步）
        /// </summary>
        public SyncStatus SyncStatus { get; set; }

        /// <summary>
        /// 显示类型（进度条/计数等）
        /// </summary>
        public string DisplayType { get; set; }

        /// <summary>
        /// 离线进度是否已处理
        /// </summary>
        public bool OfflineProgressHandled { get; set; }
    }
}
