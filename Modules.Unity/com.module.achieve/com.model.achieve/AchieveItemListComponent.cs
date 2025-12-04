using ECS;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成项列表组件
    /// </summary>
    public class AchieveItemListComponent : EcsComponent
    {
        /// <summary>达成项列表</summary>
        public List<AchieveItem> Items { get; set; } = new List<AchieveItem>();
    }
}
