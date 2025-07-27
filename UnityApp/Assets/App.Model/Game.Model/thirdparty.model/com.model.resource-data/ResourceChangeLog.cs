using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.ResourceData
{
    /// <summary>
    /// 资源变更记录实体
    /// </summary>
    public class ResourceChangeLog : EcsEntity
    {
        /// <summary>
        /// 资源变更时间
        /// </summary>
        public DateTime ChangeTime { get; set; }

        /// <summary>
        /// 资源变更类型
        /// </summary>
        public ResourceChangeType ChangeType { get; set; }

        /// <summary>
        /// 资源变更数值
        /// </summary>
        public int ChangeValue { get; set; }

        /// <summary>
        /// 资源变更原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// 资源所属者Id
        /// </summary>
        public long OwnerId { get; set; }
    }
}
