using ECS;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成项列表系统
    /// </summary>
    public class AchieveItemListSystem : AComponentSystem<EcsEntity, AchieveItemListComponent>
    {
        /// <summary>
        /// 添加达成项
        /// </summary>
        /// <param name="entity">拥有列表组件的实体</param>
        /// <param name="item">达成项实体</param>
        public static void AddItem(EcsEntity entity, AchieveItem item)
        {
            var component = entity.GetComponent<AchieveItemListComponent>();
            if (!component.Items.Contains(item))
            {
                component.Items.Add(item);
            }
        }

        /// <summary>
        /// 移除达成项
        /// </summary>
        /// <param name="entity">拥有列表组件的实体</param>
        /// <param name="item">达成项实体</param>
        public static void RemoveItem(EcsEntity entity, AchieveItem item)
        {
            var component = entity.GetComponent<AchieveItemListComponent>();
            if (component.Items.Contains(item))
            {
                component.Items.Remove(item);
            }
        }

        /// <summary>
        /// 获取所有达成项
        /// </summary>
        /// <param name="entity">拥有列表组件的实体</param>
        /// <returns>达成项列表</returns>
        public static List<AchieveItem> GetItems(EcsEntity entity)
        {
            var component = entity.GetComponent<AchieveItemListComponent>();
            return component.Items;
        }
    }
}
