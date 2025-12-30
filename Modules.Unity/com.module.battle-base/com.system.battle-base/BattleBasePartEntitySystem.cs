using ECS;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地部件实体系统，负责部件创建与数据初始化。
    /// </summary>
    public class BattleBasePartEntitySystem : AEntitySystem<BattleBasePartEntity>
    {
        /// <summary>
        /// 创建部件实体并注册到父战斗基地。
        /// </summary>
        /// <param name="parent">所属战斗基地实体</param>
        /// <param name="configId">部件配置Id</param>
        /// <param name="type">部件类型</param>
        /// <param name="quality">部件品级</param>
        /// <param name="energyCost">能量消耗</param>
        /// <returns>创建完成的部件实体</returns>
        public static BattleBasePartEntity Create(BattleBaseEntity parent, int configId, BattleBasePartType type, BattleBaseQuality quality, float energyCost)
        {
            var part = parent.AddChild<BattleBasePartEntity>(entity =>
            {
                entity.ConfigId = configId;
                SetPartData(entity, type, quality, energyCost);
            });

            BattleBasePartListSystem.AddPart(parent, part);
            return part;
        }

        /// <summary>
        /// 初始化部件数据。
        /// </summary>
        /// <param name="entity">部件实体</param>
        /// <param name="type">部件类型</param>
        /// <param name="quality">部件品级</param>
        /// <param name="energyCost">能量消耗</param>
        public static void SetPartData(BattleBasePartEntity entity, BattleBasePartType type, BattleBaseQuality quality, float energyCost)
        {
            entity.PartType = type;
            entity.Quality = quality;
            entity.EnergyCost = energyCost;
        }
    }
}
