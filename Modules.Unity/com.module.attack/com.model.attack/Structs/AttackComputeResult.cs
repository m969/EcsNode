using System;

namespace ECSGame.AttackModule
{
    public struct AttackComputeResult
    {
        public bool Success;
        public int FinalDamage; // Success == true 时有效
        public DamageFailReason FailReason; // Success == false 时有效
    }
}
