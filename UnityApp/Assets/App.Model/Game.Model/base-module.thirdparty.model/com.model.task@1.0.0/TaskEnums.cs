using ECS;
using System;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        Inactive = 0,
        InProgress = 1,
        Claimable = 2,
        Claimed = 3,
    }

    /// <summary>
    /// 任务激活方式
    /// </summary>
    public enum TaskActivateMode
    {
        Manual = 0,
    }
}
