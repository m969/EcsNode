using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 提供对区域列表组件的访问与维护功能。
    /// </summary>
    public class AreaListSystem : AComponentSystem<EcsEntity, AreaListComponent>
    {
        /// <summary>
        /// 将区域添加到系统管理的列表与字典中。
        /// </summary>
        /// <param name="world">区域所在的世界实体。</param>
        /// <param name="area">待添加的区域。</param>
        public static void AddArea(EcsEntity world, Area area)
        {
            world.GetComponent<AreaListComponent>().AreaList.Add(area);
            world.GetComponent<AreaListComponent>().AreaDict.Add(area.Id, area);
        }

        /// <summary>
        /// 从系统管理的列表与字典中移除区域。
        /// </summary>
        /// <param name="world">区域所在的世界实体。</param>
        /// <param name="area">待移除的区域。</param>
        public static void RemoveArea(EcsEntity world, Area area)
        {
            world.GetComponent<AreaListComponent>().AreaList.Remove(area);
            world.GetComponent<AreaListComponent>().AreaDict.Remove(area.Id);
        }

        /// <summary>
        /// 根据区域标识获取区域实例。
        /// </summary>
        /// <param name="world">区域所在的世界实体。</param>
        /// <param name="areaId">区域唯一标识。</param>
        /// <returns>找到的区域；若不存在则为 null。</returns>
        public static Area GetArea(EcsEntity world, long areaId)
        {
            world.GetComponent<AreaListComponent>().AreaDict.TryGetValue(areaId, out var area);
            return area;
        }
    }
}
