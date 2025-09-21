using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成类型枚举
    /// </summary>
    public enum AchieveType
    {
        /// <summary>一次性达成</summary>
    OneTime,
    /// <summary>可重复达成</summary>
    Repeatable
    }

    /// <summary>
    /// 达成状态枚举
    /// </summary>
    public enum AchieveStatus
    {
        /// <summary>未开始</summary>
        NotStarted,
        /// <summary>进行中</summary>
    InProgress,
    /// <summary>已完成</summary>
    Completed
    }
}
