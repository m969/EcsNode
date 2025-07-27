using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.ResourceData
{
    /// <summary>
    /// 资源变更组件
    /// </summary>
    public class ResourceChangeComponent : EcsComponent
    {
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
        /// 资源变更时间
        /// </summary>
        public DateTime ChangeTime { get; set; }
    }
}
