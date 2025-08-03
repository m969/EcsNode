using ECS;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成条件实体
    /// </summary>
    public class AchieveCondition : EcsEntity
    {
        /// <summary>条件类型</summary>
        public string ConditionType { get; set; }
        /// <summary>目标值</summary>
        public int TargetValue { get; set; }
        /// <summary>当前进度值</summary>
        public int CurrentValue { get; set; }
        /// <summary>参数集合</summary>
        public Dictionary<string, object> Parameters { get; set; }
        /// <summary>是否已满足</summary>
        public bool IsSatisfied { get; set; }
    }
}