using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AttackModule
{
    // 组件：伤害计算相关数据（仅数据）
    public class AttackDamageComponent : EcsComponent
    {
        public int BaseDamage { get; set; }
        public DamageFormulaType Formula { get; set; }
    }
}
