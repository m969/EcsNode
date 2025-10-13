using System;

namespace ECSGame.AttackModule
{
    public struct AttackContext
    {
        public long AttackerId;
        public long TargetId;
        public int ConfigId;
        public long StartTimeMs;
    }
}
