using ECS;
using System;

namespace ECSGame.Module.Player
{
    /// <summary>
    /// PlayerListComponent 组件系统：集合管理与相关静态业务方法。
    /// </summary>
    public class PlayerListSystem : AComponentSystem<Player, PlayerListComponent>,
        IAwake<Player, PlayerListComponent>, IInit<Player, PlayerListComponent>, IAfterInit<Player, PlayerListComponent>,
        IEnable<Player, PlayerListComponent>, IDisable<Player, PlayerListComponent>, IDestroy<Player, PlayerListComponent>
    {
        /// <summary>
        /// 将玩家实体添加到集合。
        /// </summary>
        /// <param name="entity">上下文实体，通常为拥有 PlayerListComponent 的实体</param>
        /// <param name="id">玩家 Id</param>
        /// <param name="player">玩家实体</param>
        /// <returns>添加是否成功（若已存在则返回 false）</returns>
        public static bool Add(EcsEntity entity, long id, Player player)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            if (c.Players.ContainsKey(id)) return false;
            c.Players.Add(id, player);
            entity.Dispatch<IOnPlayerCreated>(s => s.OnPlayerCreated(entity, player));
            return true;
        }

        /// <summary>
        /// 从集合中移除指定 Id 的玩家实体；若存在触发移除事件。
        /// </summary>
        /// <param name="entity">上下文实体</param>
        /// <param name="id">玩家 Id</param>
        /// <returns>是否存在并移除</returns>
        public static bool Remove(EcsEntity entity, long id)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            if (!c.Players.TryGetValue(id, out var player)) return false;
            c.Players.Remove(id);
            entity.Dispatch<IOnPlayerRemoved>(s => s.OnPlayerRemoved(entity, id));
            return true;
        }

        /// <summary>
        /// 按 Id 获取玩家实体；不存在返回 null。
        /// </summary>
        public static Player? Get(EcsEntity entity, long id)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            return c.Players.TryGetValue(id, out var player) ? player : null;
        }

        /// <summary>
        /// 集合中是否包含指定 Id。
        /// </summary>
        public static bool Contains(EcsEntity entity, long id)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            return c.Players.ContainsKey(id);
        }

        /// <summary>
        /// 返回玩家数量（优先从组件派生字段获取）。
        /// </summary>
        public static int Count(EcsEntity entity)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            return c.Count;
        }

        /// <summary>
        /// 遍历集合中所有玩家实体并执行回调。
        /// </summary>
        public static void ForEach(EcsEntity entity, Action<Player> action)
        {
            var c = entity.GetComponent<PlayerListComponent>();
            foreach (var kv in c.Players)
            {
                action(kv.Value);
            }
        }

    public void Awake(Player owner, PlayerListComponent c) { }
    public void Init(Player owner, PlayerListComponent c) { }
    public void AfterInit(Player owner, PlayerListComponent c) { }
    public void Enable(Player owner, PlayerListComponent c) { }
    public void Disable(Player owner, PlayerListComponent c) { }
    public void Destroy(Player owner, PlayerListComponent c) { }
    }
}
