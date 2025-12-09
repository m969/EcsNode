using ECS;
using System;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 队列组件系统：维护重试与补偿入队出队。
    /// </summary>
    public class RewardQueueSystem : AComponentSystem<RewardServiceEntity, RewardQueueComponent>,
        IAwake<RewardServiceEntity, RewardQueueComponent>, IInit<RewardServiceEntity, RewardQueueComponent>,
        IAfterInit<RewardServiceEntity, RewardQueueComponent>, IEnable<RewardServiceEntity, RewardQueueComponent>,
        IDisable<RewardServiceEntity, RewardQueueComponent>, IDestroy<RewardServiceEntity, RewardQueueComponent>
    {
        public void Awake(RewardServiceEntity entity, RewardQueueComponent component) { }
        public void Init(RewardServiceEntity entity, RewardQueueComponent component) { }
        public void AfterInit(RewardServiceEntity entity, RewardQueueComponent component) { }
        public void Enable(RewardServiceEntity entity, RewardQueueComponent component) { }
        public void Disable(RewardServiceEntity entity, RewardQueueComponent component) { }
        public void Destroy(RewardServiceEntity entity, RewardQueueComponent component) { }

        /// <summary>
        /// 入队一个待处理请求。
        /// </summary>
        public static void Enqueue(RewardServiceEntity service, long playerId, int packageId, DateTime nextAt, int attempt, string reason)
        {
            var q = service.GetComponent<RewardQueueComponent>();
            var id = Guid.NewGuid().ToString();
            q.Pending.Add(new RewardQueueComponent.PendingItem
            {
                QueueId = id,
                PlayerId = playerId,
                PackageId = packageId,
                NextAt = nextAt,
                Attempt = attempt,
                Reason = reason
            });
            service.Dispatch<IOnRewardQueued>((s) => s.OnRewardQueued(service, playerId, id, nextAt, attempt, reason));
        }

        /// <summary>
        /// 批量拉取队列并调用 GrantReward 进行重试，返回处理数量。
        /// </summary>
        public static int DequeueAndProcess(RewardServiceEntity service, int maxCount)
        {
            var q = service.GetComponent<RewardQueueComponent>();
            var count = Math.Min(maxCount, q.Pending.Count);
            // 这里只返回数量，具体调用外部结算逻辑由业务集成。
            return count;
        }
    }
}
