using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.ResourceDataModule
{
    /// <summary>
    /// 资源数据组件
    /// </summary>
    public class ResourceDataComponent : EcsComponent
    {
        /// <summary>
        /// 各资源类型对应的资源数值
        /// </summary>
        public Dictionary<int, int> ResourceValues { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// 上次同步资源数据的时间
        /// </summary>
        public DateTime LastSyncTime { get; set; } = DateTime.MinValue;
    }
}
