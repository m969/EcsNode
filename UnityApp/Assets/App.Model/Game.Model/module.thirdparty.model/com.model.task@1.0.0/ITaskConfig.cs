using ECS;
using System;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// 任务配置接口
    /// </summary>
    public interface ITaskConfig
    {
        int Id { get; }
        string Key { get; }
        string Name { get; }
        string Desc { get; }
        int AchieveItemId { get; }
        int RewardConfigId { get; }
    }
}
