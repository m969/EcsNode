using ECS;
using System;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// 任务实例实体（仅承载属性）
    /// </summary>
    public partial class TaskItem : EcsEntity
    {
        public int TaskId { get; set; }
        public int ConfigId { get; set; }
        public int AchieveItemId { get; set; }
        public TaskState State { get; set; }
        public float Progress { get; set; }
        public bool RewardClaimed { get; set; }
        public TaskActivateMode ActivateMode { get; set; }
        public long ActivatedAt { get; set; }
        public long CompletedAt { get; set; }
        public long ClaimedAt { get; set; }
        public int RewardConfigId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
    }
}
