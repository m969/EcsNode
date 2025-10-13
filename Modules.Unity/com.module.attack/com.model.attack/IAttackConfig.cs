using System;

namespace ECSGame.AttackModule
{
    // 配置接口：一次普攻的静态参数（只读属性）
    public interface IAttackConfig
    {
        int Id { get; }
        int BaseDamage { get; }
        int WindupDurationMs { get; }
        int ActiveDurationMs { get; }
        int RecoveryDurationMs { get; }
        DamageFormulaType DamageFormula { get; }
    }
}
