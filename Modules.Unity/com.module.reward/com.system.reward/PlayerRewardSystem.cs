using ECS;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 玩家奖励实体系统。
    /// </summary>
    public class PlayerRewardSystem : AEntitySystem<PlayerRewardEntity>,
        IAwake<PlayerRewardEntity>, IInit<PlayerRewardEntity>, IAfterInit<PlayerRewardEntity>,
        IEnable<PlayerRewardEntity>, IDisable<PlayerRewardEntity>, IDestroy<PlayerRewardEntity>
    {
        public void Awake(PlayerRewardEntity self) { }
        public void Init(PlayerRewardEntity self) { }
        public void AfterInit(PlayerRewardEntity self) { }
        public void Enable(PlayerRewardEntity self) { }
        public void Disable(PlayerRewardEntity self) { }
        public void Destroy(PlayerRewardEntity self) { }
    }
}
