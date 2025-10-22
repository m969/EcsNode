using System.Collections.Generic;
using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>维护候选目标及其评分数据。</summary>
    public class TargetCandidatesComponent : EcsComponent
    {
        /// <summary>候选目标实体 Id 列表。</summary>
        public List<long> CandidateIds { get; set; } = new List<long>();

        /// <summary>候选 Id 到评分的映射。</summary>
        public Dictionary<long, float> Id2Score { get; set; } = new Dictionary<long, float>();

        /// <summary>当前选中的目标 Id。</summary>
        public long SelectedId { get; set; }

        /// <summary>评分使用的优先级规则。</summary>
        public IPriorityRuleConfig? Rule { get; set; }
    }
}
