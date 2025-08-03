using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成项配置接口，用于配置达成项的基础信息
    /// </summary>
    public interface IAchieveItemConfig
    {
        /// <summary>达成项唯一标识</summary>
        int Id { get; }
        
        /// <summary>达成名称</summary>
        string Name { get; }
        
        /// <summary>达成描述</summary>
        string Description { get; }
        
        /// <summary>达成类型</summary>
        AchieveType Type { get; }
        
        /// <summary>优先级</summary>
        int Priority { get; }
        
        /// <summary>是否激活</summary>
        bool IsActive { get; }
        
        /// <summary>前置条件（连锁性达成需要）</summary>
        List<int> Prerequisites { get; }
        
        /// <summary>有效期（限时达成，秒数）</summary>
        long ValidityPeriod { get; }
    }
}
