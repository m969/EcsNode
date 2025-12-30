using ECS;
using System;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地实体系统，提供基础构建与能量、部件管理逻辑。
    /// </summary>
    public class BattleBaseEntitySystem : AEntitySystem<BattleBaseEntity>
    {
        private const float BaseEnergyPerLevel = 100f;

        /// <summary>
        /// 创建战斗基地实体，并初始化基础属性与部件列表组件。
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="configId">配置Id</param>
        /// <param name="quality">品级</param>
        /// <param name="level">级别</param>
        /// <param name="energyRecoveryRate">能量恢复率</param>
        /// <returns>创建完成的战斗基地实体</returns>
        public static BattleBaseEntity Create(EcsEntity parent, int configId, BattleBaseQuality quality, int level, float energyRecoveryRate)
        {
            var entity = parent.AddChild<BattleBaseEntity>(e =>
            {
                e.ConfigId = configId;
                e.Quality = quality;
                e.Level = level;
                e.EnergyRecoveryRate = energyRecoveryRate;
                e.MaxEnergy = CalculateMaxEnergy(level);
                e.CurrentEnergy = e.MaxEnergy;
            });

            entity.AddComponent<BattleBasePartListComponent>(_ => { });
            BattleBaseListSystem.AddBattleBase(parent, entity);
            return entity;
        }

        private static float CalculateMaxEnergy(int level)
        {
            return level * BaseEnergyPerLevel;
        }

        /// <summary>
        /// 设置品级。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="quality">品级</param>
        public static void SetQuality(BattleBaseEntity entity, BattleBaseQuality quality)
        {
            entity.Quality = quality;
        }

        /// <summary>
        /// 设置级别并更新能量上限。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="level">新的级别</param>
        public static void SetLevel(BattleBaseEntity entity, int level)
        {
            var oldLevel = entity.Level;
            entity.Level = level;
            entity.MaxEnergy = CalculateMaxEnergy(level);
            if (entity.CurrentEnergy > entity.MaxEnergy)
            {
                entity.CurrentEnergy = entity.MaxEnergy;
            }

            entity.Dispatch<IBattleBaseLevelChangedHandler>(handler => handler.OnBattleBaseLevelChanged(entity, oldLevel, level));
            entity.Dispatch<IBattleBaseEnergyChangedHandler>(handler => handler.OnBattleBaseEnergyChanged(entity, entity.CurrentEnergy, entity.MaxEnergy));
        }

        /// <summary>
        /// 消耗能量，返回是否成功。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="amount">能量消耗值</param>
        /// <returns>是否消耗成功</returns>
        public static bool ConsumeEnergy(BattleBaseEntity entity, float amount)
        {
            if (entity.CurrentEnergy < amount)
            {
                return false;
            }

            entity.CurrentEnergy -= amount;
            entity.Dispatch<IBattleBaseEnergyChangedHandler>(handler => handler.OnBattleBaseEnergyChanged(entity, entity.CurrentEnergy, entity.MaxEnergy));
            return true;
        }

        /// <summary>
        /// 恢复能量并派发能量变化事件。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="amount">恢复值</param>
        public static void RecoverEnergy(BattleBaseEntity entity, float amount)
        {
            var newEnergy = entity.CurrentEnergy + amount;
            entity.CurrentEnergy = Math.Min(newEnergy, entity.MaxEnergy);
            entity.Dispatch<IBattleBaseEnergyChangedHandler>(handler => handler.OnBattleBaseEnergyChanged(entity, entity.CurrentEnergy, entity.MaxEnergy));
        }

        /// <summary>
        /// 添加部件，需满足品级限制。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="partConfigId">部件配置Id</param>
        /// <param name="type">部件类型</param>
        /// <param name="quality">部件品级</param>
        /// <param name="energyCost">部件能量消耗</param>
        /// <returns>创建完成的部件实体，若品级不满足则返回null</returns>
        public static BattleBasePartEntity AddPart(BattleBaseEntity entity, int partConfigId, BattleBasePartType type, BattleBaseQuality quality, float energyCost)
        {
            if (quality > entity.Quality)
            {
                return null;
            }

            var part = BattleBasePartEntitySystem.Create(entity, partConfigId, type, quality, energyCost);
            entity.Dispatch<IBattleBasePartChangedHandler>(handler => handler.OnBattleBasePartChanged(entity, part, true));
            return part;
        }

        /// <summary>
        /// 移除部件并派发事件。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="partId">部件Id</param>
        public static void RemovePart(BattleBaseEntity entity, long partId)
        {
            var part = BattleBasePartListSystem.GetPart(entity, partId);
            if (part == null)
            {
                return;
            }

            BattleBasePartListSystem.RemovePart(entity, partId);
            entity.RemoveChild(part);
            entity.Dispatch<IBattleBasePartChangedHandler>(handler => handler.OnBattleBasePartChanged(entity, part, false));
        }
    }
}
