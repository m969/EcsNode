using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成条件配置接口
    /// </summary>
    public interface IAchieveConditionConfig
    {
        /// <summary>唯一标识</summary>
        int Id { get; }
        /// <summary>辅助名称标识</summary>
        string Key { get; }
        /// <summary>条件类型（如数值型等）</summary>
        string ConditionType { get; }
        /// <summary>目标值</summary>
        int TargetValue { get; }
        /// <summary>参数集合</summary>
        Dictionary<string, object> Parameters { get; }
    }
}
