using ECS;
using System.Collections.Generic;

namespace ECSGame.BattleBase
{
    /// <summary>
    /// 战斗基地实体
    /// 用途：作为玩家在战斗中的核心载体（战舰），承载属性、能量和部件。
    /// </summary>
    public class BattleBaseEntity : EcsEntity
    {
        /// <summary>
        /// 品级（下品、中品、上品、极品、仙品）
        /// </summary>
        public BattleBaseQuality Quality;

        /// <summary>
        /// 级别（1-5级）
        /// </summary>
        public int Level;

        /// <summary>
        /// 当前能量值
        /// </summary>
        public float CurrentEnergy;

        /// <summary>
        /// 能量上限（受级别影响）
        /// </summary>
        public float MaxEnergy;

        /// <summary>
        /// 能量恢复率
        /// </summary>
        public float EnergyRecoveryRate;
    }
}
