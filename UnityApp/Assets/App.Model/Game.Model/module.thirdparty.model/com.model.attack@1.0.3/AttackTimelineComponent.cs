using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AttackModule
{
    // 组件：时间轴/阶段推进能力（仅数据）
    public class AttackTimelineComponent : EcsComponent
    {
        public int WindupDurationMs { get; set; }
        public int ActiveDurationMs { get; set; }
        public int RecoveryDurationMs { get; set; }
        public long CreatedTimeMs { get; set; }
        public long CurrentPhaseStartMs { get; set; }
        public AttackPhase CurrentPhase { get; set; }
        public bool HitEmitted { get; set; }
    }
}
