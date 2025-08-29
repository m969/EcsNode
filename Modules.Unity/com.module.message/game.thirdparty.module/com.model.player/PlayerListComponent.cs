using ECS;
using System.Collections.Generic;

namespace ECSGame.Module.Player
{
    /// <summary>
    /// 集中存储与管理 Player 的集合，支持查找、遍历与计数。
    /// </summary>
    public class PlayerListComponent : EcsComponent
    {
        /// <summary>
        /// 以实体 Id 为键存储玩家实体的字典。
        /// </summary>
        public Dictionary<long, Player> Players { get; set; } = new Dictionary<long, Player>();

        /// <summary>
        /// 玩家数量；推荐视为从 Players.Count 派生（可缓存）。
        /// </summary>
        public int Count => Players.Count;
    }
}
