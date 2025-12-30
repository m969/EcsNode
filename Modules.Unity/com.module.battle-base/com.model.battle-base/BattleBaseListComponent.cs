using ECS;
using System.Collections.Generic;

namespace ECSGame.BattleBaseModule
{
    /// <summary>
    /// 战斗基地列表组件
    /// 用于存储和管理该实体
    /// </summary>
    public class BattleBaseListComponent : EcsComponent
    {
        /// <summary>
        /// Key为实体Id，Value为实体对象
        /// </summary>
        public Dictionary<long, BattleBaseEntity> Id2Entities = new Dictionary<long, BattleBaseEntity>();

        /// <summary>
        /// Key为配置Id，Value为实体对象列表
        /// </summary>
        public Dictionary<int, List<BattleBaseEntity>> ConfigId2Entities = new Dictionary<int, List<BattleBaseEntity>>();
    }
}
