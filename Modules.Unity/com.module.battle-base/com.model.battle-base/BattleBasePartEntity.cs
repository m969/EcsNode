using ECS;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地部件实体
    /// 用途：作为战斗基地的子实体，提供具体战斗能力（武器、护盾、引擎）。
    /// </summary>
    public class BattleBasePartEntity : EcsEntity
    {
        /// <summary>
        /// 配置Id（对应战斗基地部件配置表）
        /// </summary>
        public int ConfigId;

        /// <summary>
        /// 部件类型（武器、护盾、引擎）
        /// </summary>
        public BattleBasePartType PartType;

        /// <summary>
        /// 部件品级
        /// </summary>
        public BattleBaseQuality Quality;

        /// <summary>
        /// 能量消耗值
        /// </summary>
        public float EnergyCost;
    }
}
