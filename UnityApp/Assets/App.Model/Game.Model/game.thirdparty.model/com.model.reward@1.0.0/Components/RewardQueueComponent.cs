using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 失败重试/补偿队列（轻量信息，重数据由外部持久化）。
    /// </summary>
    public class RewardQueueComponent : EcsComponent
    {
        public struct PendingItem
        {
            public string QueueId;
            public long PlayerId;
            public int PackageId;
            public DateTime NextAt;
            public int Attempt;
            public string Reason;
        }

        public List<PendingItem> Pending { get; set; } = new();
    }
}
