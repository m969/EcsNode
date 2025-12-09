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
        IInit<AchieveItem, AchieveConditionListComponent>
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

        /// <summary>
        /// 初始化达成项条件组件
        /// </summary>
        public static void InitConditions(AchieveItem entity, List<IAchieveConditionConfig> conditionConfigs)
        {
            var component = entity.GetComponent<AchieveConditionListComponent>();
            component.ConditionList = new List<AchieveCondition>();
            foreach (var config in conditionConfigs)
            {
                var condition = new AchieveCondition
                {
                    ConditionType = config.ConditionType,
                    TargetValue = config.TargetValue,
                    Parameters = config.Parameters,
                    CurrentValue = 0,
                    IsSatisfied = false
                };
                component.ConditionList.Add(condition);
            }
            CalculateProgress(component);
        }

        /// <summary>
        /// 更新特定条件进度
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <param name="conditionIndex">条件索引</param>
        /// <param name="delta">增加的进度值</param>
        public static void UpdateConditionProgress(AchieveItem entity, int conditionIndex, int delta)
        {
            var component = entity.GetComponent<AchieveConditionListComponent>();
            if (conditionIndex < 0 || conditionIndex >= component.ConditionList.Count)
                return;

            var condition = component.ConditionList[conditionIndex];
            
            // 更新单个条件
            condition.CurrentValue += delta;
            if (condition.CurrentValue >= condition.TargetValue)
            {
                condition.CurrentValue = condition.TargetValue;
                condition.IsSatisfied = true;
            }

            // 重新计算整体进度
            CalculateProgress(component);

            // 自动判定完成
            if (component.IsAllSatisfied && entity.Status != AchieveStatus.Completed)
            {
                AchieveItemSystem.UpdateStatus(entity, AchieveStatus.Completed);
            }
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
            if (component.ConditionList.Count == 0) return false;

            foreach (var condition in component.ConditionList)
            {
                if (!condition.IsSatisfied)
                    return false;
            }
            return true;
        }
    }
}
