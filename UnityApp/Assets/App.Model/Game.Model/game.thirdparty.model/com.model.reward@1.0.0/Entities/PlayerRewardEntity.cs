using ECS;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 承载玩家侧奖励历史、幂等键、每日上限等状态。
    /// </summary>
    public partial class PlayerRewardEntity : EcsEntity
    {
        /// <summary>
        /// 玩家Id，镜像实体Id不可依赖于此，单独保留更清晰。
        /// </summary>
        public long PlayerId { get; set; }
    }
}
