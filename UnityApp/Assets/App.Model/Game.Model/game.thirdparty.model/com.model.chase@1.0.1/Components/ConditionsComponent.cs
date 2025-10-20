using System.Collections.Generic;
using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>集中存放追踪的启动与停止条件。</summary>
    public class ConditionsComponent : EcsComponent
    {
        /// <summary>当前生效的启动条件列表。</summary>
        public List<IStartConditionConfig> StartConditions { get; set; } = new List<IStartConditionConfig>();

        /// <summary>当前生效的停止条件列表。</summary>
        public List<IStopConditionConfig> StopConditions { get; set; } = new List<IStopConditionConfig>();
    }
}
