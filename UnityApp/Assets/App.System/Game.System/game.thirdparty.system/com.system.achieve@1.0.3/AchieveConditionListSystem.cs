using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成条件列表系统
    /// </summary>
    public class AchieveConditionListSystem : AComponentSystem<AchieveItem, AchieveConditionListComponent>,
        IAwake<AchieveItem, AchieveConditionListComponent>,
        IInit<AchieveItem, AchieveConditionListComponent>,
        IAfterInit<AchieveItem, AchieveConditionListComponent>,
        IEnable<AchieveItem, AchieveConditionListComponent>,
        IDisable<AchieveItem, AchieveConditionListComponent>,
        IDestroy<AchieveItem, AchieveConditionListComponent>
    {
        void IAwake<AchieveItem, AchieveConditionListComponent>.Awake(AchieveItem entity, AchieveConditionListComponent component)
        {
            component.ConditionList = new List<AchieveCondition>();
            component.TotalProgress = 0;
            component.CurrentProgress = 0;
            component.ProgressPercentage = 0f;
            component.IsAllSatisfied = false;
        }

        void IInit<AchieveItem, AchieveConditionListComponent>.Init(AchieveItem entity, AchieveConditionListComponent component)
        {
            CalculateProgress(component);
        }

        void IAfterInit<AchieveItem, AchieveConditionListComponent>.AfterInit(AchieveItem entity, AchieveConditionListComponent component) { }
        void IEnable<AchieveItem, AchieveConditionListComponent>.Enable(AchieveItem entity, AchieveConditionListComponent component) { }
        void IDisable<AchieveItem, AchieveConditionListComponent>.Disable(AchieveItem entity, AchieveConditionListComponent component) { }
        void IDestroy<AchieveItem, AchieveConditionListComponent>.Destroy(AchieveItem entity, AchieveConditionListComponent component) { }

        /// <summary>
        /// 初始化达成项条件组件
        /// </summary>
        public static void InitConditions(AchieveItem entity, List<IAchieveConditionConfig> conditionConfigs)
        {
            var component = entity.GetComponent<AchieveConditionListComponent>();
            component.ConditionList = new List<AchieveCondition>();
            foreach (var config in conditionConfigs)
            {
                var condition = entity.AddChild<AchieveCondition>(e =>
                {
                    e.ConditionType = config.ConditionType;
                    e.TargetValue = config.TargetValue;
                    e.Parameters = config.Parameters;
                    e.CurrentValue = 0;
                    e.IsSatisfied = false;
                });
                component.ConditionList.Add(condition);
            }
            CalculateProgress(component);
        }

        /// <summary>
        /// 计算进度
        /// </summary>
        public static void CalculateProgress(AchieveConditionListComponent component)
        {
            int total = 0;
            int current = 0;
            foreach (var condition in component.ConditionList)
            {
                total += condition.TargetValue;
                current += condition.CurrentValue;
            }
            component.TotalProgress = total;
            component.CurrentProgress = current;
            component.ProgressPercentage = total > 0 ? (float)current / total : 0f;
            component.IsAllSatisfied = IsAllSatisfied(component);
        }

        /// <summary>
        /// 是否全部满足
        /// </summary>
        public static bool IsAllSatisfied(AchieveConditionListComponent component)
        {
            foreach (var condition in component.ConditionList)
            {
                if (!condition.IsSatisfied)
                    return false;
            }
            return true;
        }
    }
}
