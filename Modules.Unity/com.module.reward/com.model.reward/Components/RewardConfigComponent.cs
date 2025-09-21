using ECS;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 缓存奖励条目与包配置、掉落表，提供快速查询。
    /// </summary>
    public class RewardConfigComponent : EcsComponent
    {
        public Dictionary<int, IRewardEntryConfig> Entries { get; set; } = new();
        public Dictionary<int, IRewardPackageConfig> Packages { get; set; } = new();
        public Dictionary<int, IRewardLootTableConfig> LootTables { get; set; } = new();
    }
}
