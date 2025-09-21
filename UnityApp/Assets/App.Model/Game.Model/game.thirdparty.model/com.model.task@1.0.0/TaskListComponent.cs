using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// 任务列表与索引组件（仅数据）
    /// </summary>
    public class TaskListComponent : EcsComponent
    {
    public Dictionary<int, TaskItem> Id2Entities { get; set; } = new Dictionary<int, TaskItem>();
    public Dictionary<int, List<TaskItem>> ConfigId2Entities { get; set; } = new Dictionary<int, List<TaskItem>>();
    public Dictionary<int, List<int>> AchieveItemId2TaskIds { get; set; } = new Dictionary<int, List<int>>();
    public Queue<int> RecentChangedTaskIds { get; set; } = new Queue<int>();
    }
}
