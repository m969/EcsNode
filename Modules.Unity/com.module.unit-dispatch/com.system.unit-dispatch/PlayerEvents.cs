using ECS;

namespace ECSGame.Module.Player
{
    /// <summary>
    /// 玩家被创建时派发；常用于统计与外部订阅扩展。
    /// </summary>
    public interface IOnPlayerCreated : IDispatch
    {
        void OnPlayerCreated(EcsEntity context, Player player);
    }

    /// <summary>
    /// 玩家被移除时派发；常用于清理、记录与 UI 更新。
    /// </summary>
    public interface IOnPlayerRemoved : IDispatch
    {
        void OnPlayerRemoved(EcsEntity context, long playerId);
    }
}
