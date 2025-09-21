using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成项配置接口，用于配置达成项的基础信息
    /// </summary>
    public interface IAchieveItemConfig
    {
        /// <summary>唯一标识</summary>
        int Id { get; }
        /// <summary>辅助名称标识</summary>
        string Key { get; }
        /// <summary>名称</summary>
        string Name { get; }
        /// <summary>描述</summary>
        string Description { get; }
        /// <summary>类型（如一次性/重复性）</summary>
        AchieveType Type { get; }
        /// <summary>优先级</summary>
        int Priority { get; }
    }
}
