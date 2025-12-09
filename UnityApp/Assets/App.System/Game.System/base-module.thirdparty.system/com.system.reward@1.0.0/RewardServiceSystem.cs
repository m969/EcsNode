using ECS;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 奖励服务实体系统：负责创建根实体与玩家实体。
    /// </summary>
    public class RewardServiceSystem : AEntitySystem<RewardServiceEntity>,
        IAwake<RewardServiceEntity>, IInit<RewardServiceEntity>, IAfterInit<RewardServiceEntity>,
        IEnable<RewardServiceEntity>, IDisable<RewardServiceEntity>, IDestroy<RewardServiceEntity>
    {
        public void Awake(RewardServiceEntity self) { }
        public void Init(RewardServiceEntity self) { }
        public void AfterInit(RewardServiceEntity self) { }
        public void Enable(RewardServiceEntity self) { }
        public void Disable(RewardServiceEntity self) { }
        public void Destroy(RewardServiceEntity self) { }

        /// <summary>
        /// 创建模块根实体，挂载配置组件并完成初始化。
        /// </summary>
        public static RewardServiceEntity Create(EcsEntity parent, IRewardConfigProvider provider)
        {
            var service = parent.AddChild<RewardServiceEntity>((e) => { });
            service.AddComponent<RewardConfigComponent>((c) => { });
            service.AddComponent<RewardQueueComponent>((c) => { });
            service.AddComponent<RewardAuditComponent>((c) => { });
            RewardConfigSystem.Reload(service, provider);
            return service;
        }

        /// <summary>
        /// 获取/创建玩家奖励实体，确保挂载历史组件。
        /// </summary>
        public static PlayerRewardEntity GetPlayerReward(EcsEntity parentOrService, long playerId)
        {
            var player = parentOrService.AddChild<PlayerRewardEntity>((e) => { e.PlayerId = playerId; });
            player.AddComponent<RewardHistoryComponent>((c) => { });
            return player;
        }
    }
}
