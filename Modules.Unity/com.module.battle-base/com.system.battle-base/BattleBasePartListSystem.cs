using ECS;
using System.Collections.Generic;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地部件列表组件系统，提供部件查询与注册。
    /// </summary>
    public class BattleBasePartListSystem : AComponentSystem<BattleBaseEntity, BattleBasePartListComponent>
    {
        /// <summary>
        /// 按Id获取部件。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="id">部件Id</param>
        /// <returns>部件实体，未找到时返回null</returns>
        public static BattleBasePartEntity GetPart(BattleBaseEntity entity, long id)
        {
            var component = entity.GetComponent<BattleBasePartListComponent>();
            component.Id2Entities.TryGetValue(id, out var part);
            return part;
        }

        /// <summary>
        /// 将部件注册到战斗基地的部件列表组件。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="part">部件实体</param>
        public static void AddPart(BattleBaseEntity entity, BattleBasePartEntity part)
        {
            var component = entity.GetComponent<BattleBasePartListComponent>();
            component.Id2Entities[part.Id] = part;

            if (!component.ConfigId2Entities.TryGetValue(part.ConfigId, out var list))
            {
                list = new List<BattleBasePartEntity>();
                component.ConfigId2Entities[part.ConfigId] = list;
            }

            list.Add(part);
        }

        /// <summary>
        /// 从战斗基地的部件列表中移除部件。
        /// </summary>
        /// <param name="entity">战斗基地实体</param>
        /// <param name="partId">部件Id</param>
        public static void RemovePart(BattleBaseEntity entity, long partId)
        {
            var component = entity.GetComponent<BattleBasePartListComponent>();
            if (!component.Id2Entities.TryGetValue(partId, out var part))
            {
                return;
            }

            component.Id2Entities.Remove(partId);

            var list = component.ConfigId2Entities[part.ConfigId];
            list.Remove(part);
            if (list.Count == 0)
            {
                component.ConfigId2Entities.Remove(part.ConfigId);
            }
        }
    }
}
