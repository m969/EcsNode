using ECS;
using System;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// TaskItem 实体系统：负责单个任务的状态流转与结算调用
    /// </summary>
    public class TaskItemSystem : AEntitySystem<TaskItem>,
        IAwake<TaskItem>, IInit<TaskItem>, IAfterInit<TaskItem>,
        IEnable<TaskItem>, IDisable<TaskItem>, IDestroy<TaskItem>
    {
        void IAwake<TaskItem>.Awake(TaskItem entity)
        {
            entity.State = TaskState.Inactive;
            entity.Progress = 0f;
            entity.RewardClaimed = false;
            entity.ActivateMode = TaskActivateMode.Manual;
            entity.ActivatedAt = 0;
            entity.CompletedAt = 0;
            entity.ClaimedAt = 0;
        }

        void IInit<TaskItem>.Init(TaskItem entity) { }
        void IAfterInit<TaskItem>.AfterInit(TaskItem entity) { }
        void IEnable<TaskItem>.Enable(TaskItem entity) { }
        void IDisable<TaskItem>.Disable(TaskItem entity) { }
        void IDestroy<TaskItem>.Destroy(TaskItem entity) { }

        /// <summary>
        /// 创建 TaskItem 实体并初始化
        /// </summary>
        public static TaskItem Create(EcsEntity parent, ITaskConfig config)
        {
            var task = parent.AddChild<TaskItem>(e =>
            {
                e.TaskId = config.Id;
                e.ConfigId = config.Id;
                e.AchieveItemId = config.AchieveItemId;
                e.RewardConfigId = config.RewardConfigId;
                e.Key = config.Key;
                e.Name = config.Name;
                e.Desc = config.Desc;
                e.State = TaskState.Inactive;
                e.Progress = 0f;
                e.RewardClaimed = false;
                e.ActivateMode = TaskActivateMode.Manual;
                e.ActivatedAt = 0;
                e.CompletedAt = 0;
                e.ClaimedAt = 0;
            });
            return task;
        }

        /// <summary>
        /// 激活任务：Inactive → InProgress
        /// </summary>
        public static void Activate(TaskItem task)
        {
            if (task.State == TaskState.Inactive)
            {
                task.State = TaskState.InProgress;
                task.ActivatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                task.Dispatch<IOnTaskActivated>(s => s.OnTaskActivated(task, task.TaskId));
            }
        }

        /// <summary>
        /// 根据达成完成态与当前状态计算新状态
        /// </summary>
        public static TaskState EvaluateState(TaskItem task, bool completed)
        {
            if (task.State == TaskState.Claimed) return TaskState.Claimed;
            if (completed) return task.State == TaskState.InProgress ? TaskState.Claimable : task.State;
            return task.State;
        }

        /// <summary>
        /// 领取奖励：Claimable → Claimed；通过外部回调发放奖励
        /// </summary>
        public static bool Claim(TaskItem task, Func<int, bool> tryGrantReward)
        {
            if (task.State != TaskState.Claimable) return false;
            var ok = tryGrantReward(task.RewardConfigId);
            if (!ok) return false;
            task.State = TaskState.Claimed;
            task.RewardClaimed = true;
            task.ClaimedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            task.Dispatch<IOnTaskRewardClaimed>(s => s.OnTaskRewardClaimed(task, task.TaskId));
            return true;
        }

        /// <summary>
        /// 重置任务到 Inactive（默认），不修改 Achieve 进度
        /// </summary>
        public static void Reset(TaskItem task, bool resetToInactive = true)
        {
            if (resetToInactive)
            {
                task.State = TaskState.Inactive;
            }
            task.RewardClaimed = false;
            task.ActivatedAt = 0;
            task.CompletedAt = 0;
            task.ClaimedAt = 0;
            task.Dispatch<IOnTaskReset>(s => s.OnTaskReset(task, task.TaskId));
        }

        /// <summary>
        /// 获取进度（只读）
        /// </summary>
        public static float GetProgress(TaskItem task) => task.Progress;

        /// <summary>
        /// 获取状态
        /// </summary>
        public static TaskState GetState(TaskItem task) => task.State;
    }
}
