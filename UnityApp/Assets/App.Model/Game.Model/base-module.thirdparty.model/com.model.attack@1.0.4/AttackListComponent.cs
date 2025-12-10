using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AttackModule
{
    // 组件：存储与管理所有 AttackAction 的数据容器（纯数据）
    public class AttackListComponent : EcsComponent
    {
        public static readonly List<long> CachedRemoveList = new List<long>();
        public Dictionary<long, AttackAction> Id2Entities { get; set; } = new Dictionary<long, AttackAction>();
        public Dictionary<int, List<AttackAction>> ConfigId2Entities { get; set; } = new Dictionary<int, List<AttackAction>>();
        public Dictionary<long, long> AttackerId2LatestAttackId { get; set; } = new Dictionary<long, long>();
    }
}
