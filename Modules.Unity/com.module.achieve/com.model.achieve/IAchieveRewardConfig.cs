using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成奖励配置接口，用于配置奖励内容
    /// </summary>
    public interface IAchieveRewardConfig
    {
        /// <summary>唯一标识</summary>
        int Id { get; }
        /// <summary>辅助名称标识</summary>
        string Key { get; }
        /// <summary>奖励类型（虚拟货币/道具）</summary>
        RewardType RewardType { get; }
        /// <summary>物品ID</summary>
        int ItemId { get; }
        /// <summary>奖励数量</summary>
        int Amount { get; }
    }
}
