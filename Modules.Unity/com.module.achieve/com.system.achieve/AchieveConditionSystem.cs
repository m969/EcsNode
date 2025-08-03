using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成条件系统
    /// </summary>
    public class AchieveConditionSystem : AEntitySystem<AchieveCondition>, 
        IAwake<AchieveCondition>, 
        IInit<AchieveCondition>,
        IUpdate<AchieveCondition>
    {
        void IAwake<AchieveCondition>.Awake(AchieveCondition entity)
        {
            // 在实体被创建时调用
            entity.CurrentValue = 0;
            entity.IsSatisfied = false;
        }

        void IInit<AchieveCondition>.Init(AchieveCondition entity)
        {
            // 在实体初始化时调用，检查初始状态
            if (entity.CurrentValue >= entity.TargetValue)
            {
                entity.CurrentValue = entity.TargetValue;
                entity.IsSatisfied = true;
            }
        }

        void IUpdate<AchieveCondition>.Update(AchieveCondition entity)
        {
            // 每帧更新时调用，可以在这里处理需要持续检查的条件
            if (!entity.IsSatisfied && entity.CurrentValue >= entity.TargetValue)
            {
                entity.CurrentValue = entity.TargetValue;
                entity.IsSatisfied = true;
            }
        }

        /// <summary>
        /// 创建达成条件实体
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="config">条件配置</param>
        /// <returns>达成条件实体</returns>
        public static AchieveCondition Create(EcsEntity parent, IAchieveConditionConfig config)
        {
            return parent.AddChild<AchieveCondition>(e =>
            {
                e.ConditionType = config.ConditionType;
                e.TargetValue = config.TargetValue;
                e.Parameters = config.Parameters;
                e.CurrentValue = 0;
                e.IsSatisfied = false;
            });
        }

        /// <summary>
        /// 更新达成条件进度
        /// </summary>
        /// <param name="entity">达成条件实体</param>
        /// <param name="value">增加的进度值</param>
        public static void UpdateProgress(AchieveCondition entity, int value)
        {
            entity.CurrentValue += value;
            if (entity.CurrentValue >= entity.TargetValue)
            {
                entity.CurrentValue = entity.TargetValue;
                entity.IsSatisfied = true;
            }
        }

        /// <summary>
        /// 判定条件是否满足
        /// </summary>
        /// <param name="entity">达成条件实体</param>
        /// <returns>是否满足</returns>
        public static bool IsSatisfied(AchieveCondition entity)
        {
            return entity.IsSatisfied;
        }
    }
}
