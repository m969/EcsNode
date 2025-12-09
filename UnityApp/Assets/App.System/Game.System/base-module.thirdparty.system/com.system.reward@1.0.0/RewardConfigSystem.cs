using ECS;
using System.Linq;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 配置组件系统：负责加载与热更配置。
    /// </summary>
    public class RewardConfigSystem : AComponentSystem<RewardServiceEntity, RewardConfigComponent>,
        IAwake<RewardServiceEntity, RewardConfigComponent>, IInit<RewardServiceEntity, RewardConfigComponent>,
        IAfterInit<RewardServiceEntity, RewardConfigComponent>, IEnable<RewardServiceEntity, RewardConfigComponent>,
        IDisable<RewardServiceEntity, RewardConfigComponent>, IDestroy<RewardServiceEntity, RewardConfigComponent>
    {
        public void Awake(RewardServiceEntity entity, RewardConfigComponent component) { }
        public void Init(RewardServiceEntity entity, RewardConfigComponent component) { }
        public void AfterInit(RewardServiceEntity entity, RewardConfigComponent component) { }
        public void Enable(RewardServiceEntity entity, RewardConfigComponent component) { }
        public void Disable(RewardServiceEntity entity, RewardConfigComponent component) { }
        public void Destroy(RewardServiceEntity entity, RewardConfigComponent component) { }

        /// <summary>
        /// 热更/重新加载配置并完成强校验。
        /// </summary>
        public static void Reload(RewardServiceEntity service, IRewardConfigProvider provider)
        {
            var cfg = service.GetComponent<RewardConfigComponent>();
            cfg.Entries = provider.LoadEntries().ToDictionary(kv => kv.Key, kv => kv.Value);
            cfg.Packages = provider.LoadPackages().ToDictionary(kv => kv.Key, kv => kv.Value);
            cfg.LootTables = provider.LoadLootTables().ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
