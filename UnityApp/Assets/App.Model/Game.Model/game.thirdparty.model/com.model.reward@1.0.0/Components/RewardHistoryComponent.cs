using ECS;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 保存幂等键与发放流水索引，支持去重与审计查询。
    /// </summary>
    public class RewardHistoryComponent : EcsComponent
    {
        public HashSet<string> DedupeKeys { get; set; } = new();
        public List<string> TxIndex { get; set; } = new();
    }
}
