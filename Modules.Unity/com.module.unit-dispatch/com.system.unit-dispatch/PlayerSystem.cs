using ECS;

namespace ECSGame.Module.Player
{
    /// <summary>
    /// Player 实体系统：实现生命周期回调与创建/移除业务方法。
    /// 注意：系统本身为实例类，但业务方法为静态，便于调用。
    /// </summary>
    public class PlayerSystem : AEntitySystem<Player>,
        IAwake<Player>, IInit<Player>, IAfterInit<Player>, IEnable<Player>, IDisable<Player>, IDestroy<Player>
    {
        /// <summary>
        /// 创建并挂载一个 Player 到指定 parent 下；Awake 前可填充必要数据。
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <returns>创建的玩家实体</returns>
        public static Player Create(EcsEntity parent)
        {
            var player = parent.AddChild<Player>(_ => { /* 暂无参数填充 */ });
            parent.Dispatch<IOnPlayerCreated>(s => s.OnPlayerCreated(parent, player));
            return player;
        }

        /// <summary>
        /// 销毁指定玩家实体；由框架统一销毁流程处理。
        /// </summary>
        /// <param name="player">玩家实体</param>
        public static void Remove(Player player)
        {
            var parent = player.Parent;
            var id = player.Id;
            if (parent != null)
            {
                parent.RemoveChild(player);
            }
            (parent ?? player).Dispatch<IOnPlayerRemoved>(s => s.OnPlayerRemoved(parent ?? player, id));
        }

        public void Awake(Player e) { }
        public void Init(Player e) { }
        public void AfterInit(Player e) { }
        public void Enable(Player e) { }
        public void Disable(Player e) { }
        public void Destroy(Player e) { }
    }
}
