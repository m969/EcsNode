using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成项奖励系统
    /// </summary>
    public class AchieveItemRewardSystem : AComponentSystem<AchieveItem, AchieveItemRewardComponent>,
        IAwake<AchieveItem, AchieveItemRewardComponent>,
        IInit<AchieveItem, AchieveItemRewardComponent>
    {
        void IAwake<AchieveItem, AchieveItemRewardComponent>.Awake(AchieveItem entity, AchieveItemRewardComponent component)
        {
            // 在组件被创建时调用
            component.RewardList = new List<IAchieveRewardConfig>();
            component.RewardStatus = RewardStatus.NotGranted;
            component.ClaimTime = 0;
        }

        void IInit<AchieveItem, AchieveItemRewardComponent>.Init(AchieveItem entity, AchieveItemRewardComponent component)
        {
            // 在组件初始化时调用
            if (component.RewardStatus == RewardStatus.Granted && component.ClaimTime == 0)
            {
                component.ClaimTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// 初始化达成项奖励组件
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <param name="rewardConfigs">奖励配置列表</param>
        public static void InitRewards(AchieveItem entity, List<IAchieveRewardConfig> rewardConfigs)
        {
            var component = entity.GetComponent<AchieveItemRewardComponent>();
            component.RewardList = rewardConfigs;
            component.RewardStatus = RewardStatus.NotGranted;
            component.ClaimTime = 0;
        }

        /// <summary>
        /// 发放奖励
        /// </summary>
        /// <param name="component">达成项奖励组件</param>
        /// <param name="player">玩家实体</param>
        public static void GrantRewards(AchieveItemRewardComponent component, EcsEntity player)
        {
            // 奖励发放逻辑（实际发放需根据游戏实现）
            component.RewardStatus = RewardStatus.Granted;
            component.ClaimTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 更新奖励状态
        /// </summary>
        /// <param name="component">达成项奖励组件</param>
        /// <param name="status">奖励状态</param>
        public static void UpdateRewardStatus(AchieveItemRewardComponent component, RewardStatus status)
        {
            component.RewardStatus = status;
            if (status == RewardStatus.Granted)
            {
                component.ClaimTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
    }
}
