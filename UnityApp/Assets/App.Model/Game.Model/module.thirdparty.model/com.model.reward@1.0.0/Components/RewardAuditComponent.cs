using ECS;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 模块内审计缓冲，供外部订阅/落库。
    /// </summary>
    public class RewardAuditComponent : EcsComponent
    {
        public Queue<string> Logs { get; set; } = new();
    }
}
