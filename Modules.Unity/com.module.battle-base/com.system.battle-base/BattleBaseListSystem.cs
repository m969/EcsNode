using ECS;
using System.Collections.Generic;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地列表组件系统，提供战斗基地的索引与查询。
    /// </summary>
    public class BattleBaseListSystem : AComponentSystem<EcsEntity, BattleBaseListComponent>
    {
        /// <summary>
        /// 按Id获取战斗基地实体。
        /// </summary>
        /// <param name="entity">承载列表组件的实体</param>
        /// <param name="id">战斗基地Id</param>
        /// <returns>战斗基地实体</returns>
        public static BattleBaseEntity GetBattleBase(EcsEntity entity, long id)
        {
            var component = entity.GetComponent<BattleBaseListComponent>();
            return component.Id2Entities[id];
        }

        /// <summary>
        /// 根据配置Id获取战斗基地列表。
        /// </summary>
        /// <param name="entity">承载列表组件的实体</param>
        /// <param name="configId">配置Id</param>
        /// <returns>战斗基地列表</returns>
        public static List<BattleBaseEntity> GetBattleBasesByConfigId(EcsEntity entity, int configId)
        {
            var component = entity.GetComponent<BattleBaseListComponent>();
            if (!component.ConfigId2Entities.TryGetValue(configId, out var list))
            {
                list = new List<BattleBaseEntity>();
                component.ConfigId2Entities[configId] = list;
            }

            return list;
        }

        /// <summary>
        /// 将战斗基地注册到列表组件。
        /// </summary>
        /// <param name="entity">承载列表组件的实体</param>
        /// <param name="battleBase">战斗基地实体</param>
        public static void AddBattleBase(EcsEntity entity, BattleBaseEntity battleBase)
        {
            var component = entity.GetComponent<BattleBaseListComponent>();
            component.Id2Entities[battleBase.Id] = battleBase;

            var list = GetBattleBasesByConfigId(entity, battleBase.ConfigId);
            list.Add(battleBase);
        }

        /// <summary>
        /// 从列表组件移除战斗基地。
        /// </summary>
        /// <param name="entity">承载列表组件的实体</param>
        /// <param name="battleBase">待移除的战斗基地实体</param>
        public static void RemoveBattleBase(EcsEntity entity, BattleBaseEntity battleBase)
        {
            var component = entity.GetComponent<BattleBaseListComponent>();
            component.Id2Entities.Remove(battleBase.Id);

            var list = GetBattleBasesByConfigId(entity, battleBase.ConfigId);
            list.Remove(battleBase);
            if (list.Count == 0)
            {
                component.ConfigId2Entities.Remove(battleBase.ConfigId);
            }
        }
    }
}
