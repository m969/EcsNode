using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成项条件系统
    /// </summary>
    public class AchieveItemConditionSystem : AComponentSystem<AchieveItem, AchieveItemConditionComponent>,
        IAwake<AchieveItem, AchieveItemConditionComponent>,
        IInit<AchieveItem, AchieveItemConditionComponent>
    {
        void IAwake<AchieveItem, AchieveItemConditionComponent>.Awake(AchieveItem entity, AchieveItemConditionComponent component)
        {
            // 在组件被创建时调用
            component.ConditionList = new List<AchieveCondition>();
            component.TotalProgress = 0;
            component.CurrentProgress = 0;
            component.ProgressPercentage = 0f;
            component.IsAllSatisfied = false;
        }

        void IInit<AchieveItem, AchieveItemConditionComponent>.Init(AchieveItem entity, AchieveItemConditionComponent component)
        {
            // 在组件初始化时调用
            CalculateProgress(component);
        }

        /// <summary>
        /// 初始化达成项条件组件
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <param name="conditionConfigs">条件配置列表</param>
        public static void InitConditions(AchieveItem entity, List<IAchieveConditionConfig> conditionConfigs)
        {
            var component = entity.GetComponent<AchieveItemConditionComponent>();
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
        /// 计算达成项条件进度
        /// </summary>
        /// <param name="component">达成项条件组件</param>
        public static void CalculateProgress(AchieveItemConditionComponent component)
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
        /// 判定所有条件是否全部满足
        /// </summary>
        /// <param name="component">达成项条件组件</param>
        /// <returns>是否全部满足</returns>
        public static bool IsAllSatisfied(AchieveItemConditionComponent component)
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
