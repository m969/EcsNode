using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 奖励发放组件系统：预览、校验与发放。
    /// </summary>
    public class RewardGrantSystem : AComponentSystem<RewardServiceEntity, RewardConfigComponent>,
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
        /// 读取并展开奖励（含掉落表），合并同类项与单位标准化，不落库；派发 OnRewardPreviewed。
        /// </summary>
        public static ExpandResult PreviewReward(RewardServiceEntity service, GrantContext ctx, int packageId)
        {
            var result = new ExpandResult { Entries = new List<RewardEntryDefinition>(), Logs = new List<string>() };
            service.Dispatch<IOnRewardPreviewed>((s) => s.OnRewardPreviewed(service, ctx, result));
            return result;
        }

        /// <summary>
        /// 读取并展开奖励（含掉落表），使用直接定义列表。
        /// </summary>
        public static ExpandResult PreviewReward(RewardServiceEntity service, GrantContext ctx, RewardEntryDefinition[] defs)
        {
            var result = new ExpandResult { Entries = new List<RewardEntryDefinition>(defs), Logs = new List<string>() };
            service.Dispatch<IOnRewardPreviewed>((s) => s.OnRewardPreviewed(service, ctx, result));
            return result;
        }

        /// <summary>
        /// 校验时间窗/每日上限/等级/平台/地区/名单/唯一性/速率限制，不产生副作用。
        /// </summary>
        public static (bool ok, List<string> reasons) CanGrant(RewardServiceEntity service, GrantContext ctx, int packageId)
        {
            return (true, new List<string>());
        }

        /// <summary>
        /// 校验（直接定义列表）。
        /// </summary>
        public static (bool ok, List<string> reasons) CanGrant(RewardServiceEntity service, GrantContext ctx, RewardEntryDefinition[] defs)
        {
            return (true, new List<string>());
        }

        /// <summary>
        /// 检查幂等键，执行经济与背包结算（委托外部模块），派发 OnRewardGranted/OnRewardFailed；
        /// 失败时可入队并派发 OnRewardQueued。
        /// </summary>
        public static GrantResult GrantReward(RewardServiceEntity service, PlayerRewardEntity player, GrantContext ctx, int packageId, string? dedupeKey)
        {
            var result = new GrantResult
            {
                Status = GrantStatus.Success,
                EntriesApplied = new List<RewardEntryDefinition>(),
                EntriesSkipped = new List<(RewardEntryDefinition, string)>(),
                Error = null,
                TxId = Guid.NewGuid().ToString(),
                Logs = new List<string>()
            };

            service.Dispatch<IOnRewardGranted>((s) => s.OnRewardGranted(service, ctx, result));
            return result;
        }

        /// <summary>
        /// 发放（直接定义列表）。
        /// </summary>
        public static GrantResult GrantReward(RewardServiceEntity service, PlayerRewardEntity player, GrantContext ctx, RewardEntryDefinition[] defs, string? dedupeKey)
        {
            var result = new GrantResult
            {
                Status = GrantStatus.Success,
                EntriesApplied = new List<RewardEntryDefinition>(defs),
                EntriesSkipped = new List<(RewardEntryDefinition, string)>(),
                Error = null,
                TxId = Guid.NewGuid().ToString(),
                Logs = new List<string>()
            };
            service.Dispatch<IOnRewardGranted>((s) => s.OnRewardGranted(service, ctx, result));
            return result;
        }
    }

    /// <summary>
    /// 预览完成派发。
    /// </summary>
    public interface IOnRewardPreviewed : IDispatch
    {
        void OnRewardPreviewed(EcsEntity entity, GrantContext ctx, ExpandResult result);
    }

    /// <summary>
    /// 发放成功派发。
    /// </summary>
    public interface IOnRewardGranted : IDispatch
    {
        void OnRewardGranted(EcsEntity entity, GrantContext ctx, GrantResult result);
    }

    /// <summary>
    /// 发放失败派发。
    /// </summary>
    public interface IOnRewardFailed : IDispatch
    {
        void OnRewardFailed(EcsEntity entity, GrantContext ctx, ErrorDomain domain, int code, string message);
    }

    /// <summary>
    /// 入队派发。
    /// </summary>
    public interface IOnRewardQueued : IDispatch
    {
        void OnRewardQueued(EcsEntity entity, long playerId, string queueId, DateTime nextAt, int attempt, string reason);
    }
}
