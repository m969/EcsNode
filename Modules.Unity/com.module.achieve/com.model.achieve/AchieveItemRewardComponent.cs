using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成奖励组件，描述达成的奖励内容
    /// </summary>
    public class AchieveItemRewardComponent : EcsComponent
    {
        /// <summary>奖励列表</summary>
        public List<IAchieveRewardConfig> RewardList { get; set; }

        /// <summary>
        /// 奖励状态（未发放/已发放/发放失败）
        /// </summary>
        public RewardStatus RewardStatus { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public long ClaimTime { get; set; }
    }
}
