using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AttackModule
{
    // 实体：一次普攻的运行时实例，仅承载数据
    public class AttackAction : EcsEntity
    {
        public long AttackerId { get; set; }
        public long TargetId { get; set; }
        public int ConfigId { get; set; }
        public long StartTimeMs { get; set; }
    }
}
