using ECS;
using System;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成项实体
    /// </summary>
    public class AchieveItem : EcsEntity
    {
        /// <summary>名称</summary>
        public string Name { get; set; }
        /// <summary>描述</summary>
        public string Description { get; set; }
        /// <summary>类型（如一次性/重复性）</summary>
        public AchieveType Type { get; set; }
        /// <summary>状态（未开始/进行中/已完成）</summary>
        public AchieveStatus Status { get; set; }
        /// <summary>创建时间</summary>
        public long CreateTime { get; set; }
        /// <summary>完成时间</summary>
        public long CompleteTime { get; set; }
        /// <summary>优先级</summary>
        public int Priority { get; set; }
    }
}
