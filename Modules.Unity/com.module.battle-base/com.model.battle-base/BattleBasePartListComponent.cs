using ECS;
using System.Collections.Generic;

namespace ECSGame.BattleBase
{
    /// <summary>
    /// 战斗基地部件列表组件
    /// 用于存储和管理该实体
    /// </summary>
    public class BattleBasePartListComponent : EcsComponent
    {
        /// <summary>
        /// Key为实体Id，Value为实体对象
        /// </summary>
        public Dictionary<long, BattleBasePartEntity> Id2Entities = new Dictionary<long, BattleBasePartEntity>();

        /// <summary>
        /// Key为配置Id，Value为实体对象列表
        /// </summary>
        public Dictionary<long, List<BattleBasePartEntity>> ConfigId2Entities = new Dictionary<long, List<BattleBasePartEntity>>();
    }
}
