using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 提供对角色列表组件的访问与维护功能。
    /// </summary>
    public class ActorListSystem : AComponentSystem<EcsEntity, ActorListComponent>
    {
        /// <summary>
        /// 将角色添加到系统管理的列表与字典中。
        /// </summary>
        /// <param name="world">角色所在的世界实体。</param>
        /// <param name="actor">待添加的角色。</param>
        public static void AddActor(EcsEntity world, Actor actor)
        {
            world.GetComponent<ActorListComponent>().ActorList.Add(actor);
            world.GetComponent<ActorListComponent>().ActorDict.Add(actor.Id, actor);
        }

        /// <summary>
        /// 从系统管理的列表与字典中移除角色。
        /// </summary>
        /// <param name="world">角色所在的世界实体。</param>
        /// <param name="actor">待移除的角色。</param>
        public static void RemoveActor(EcsEntity world, Actor actor)
        {
            world.GetComponent<ActorListComponent>().ActorList.Remove(actor);
            world.GetComponent<ActorListComponent>().ActorDict.Remove(actor.Id);
        }

        /// <summary>
        /// 根据角色标识获取角色实例。
        /// </summary>
        /// <param name="world">角色所在的世界实体。</param>
        /// <param name="actorId">角色唯一标识。</param>
        /// <returns>找到的角色；若不存在则为 null。</returns>
        public static Actor GetActor(EcsEntity world, long actorId)
        {
            world.GetComponent<ActorListComponent>().ActorDict.TryGetValue(actorId, out var actor);
            return actor;
        }
    }
}
