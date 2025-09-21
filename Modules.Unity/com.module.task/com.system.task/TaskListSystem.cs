using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.TaskModule
{
    /// <summary>
    /// 任务列表组件系统：维护索引并提供便捷Id封装
    /// </summary>
    public class TaskListSystem : AComponentSystem<EcsEntity, TaskListComponent>,
        IAwake<EcsEntity, TaskListComponent>, IInit<EcsEntity, TaskListComponent>, IAfterInit<EcsEntity, TaskListComponent>,
        IEnable<EcsEntity, TaskListComponent>, IDisable<EcsEntity, TaskListComponent>, IDestroy<EcsEntity, TaskListComponent>
    {
        void IAwake<EcsEntity, TaskListComponent>.Awake(EcsEntity entity, TaskListComponent component)
        {
            // 字典构造由组件默认值完成
        }

        void IInit<EcsEntity, TaskListComponent>.Init(EcsEntity entity, TaskListComponent component) { }
        void IAfterInit<EcsEntity, TaskListComponent>.AfterInit(EcsEntity entity, TaskListComponent component) { }
        void IEnable<EcsEntity, TaskListComponent>.Enable(EcsEntity entity, TaskListComponent component) { }
        void IDisable<EcsEntity, TaskListComponent>.Disable(EcsEntity entity, TaskListComponent component) { }
        void IDestroy<EcsEntity, TaskListComponent>.Destroy(EcsEntity entity, TaskListComponent component) { }

        /// <summary>
        /// 加入索引
        /// </summary>
        public static bool Add(EcsEntity root, TaskItem task)
        {
            var list = root.GetComponent<TaskListComponent>();
            if (list.Id2Entities.ContainsKey(task.TaskId)) return false;
            list.Id2Entities[task.TaskId] = task;

            if (!list.ConfigId2Entities.TryGetValue(task.ConfigId, out var arr))
            {
                arr = new List<TaskItem>();
                list.ConfigId2Entities[task.ConfigId] = arr;
            }
            arr.Add(task);

            if (!list.AchieveItemId2TaskIds.TryGetValue(task.AchieveItemId, out var ids))
            {
                ids = new List<int>();
                list.AchieveItemId2TaskIds[task.AchieveItemId] = ids;
            }
            ids.Add(task.TaskId);

            list.RecentChangedTaskIds.Enqueue(task.TaskId);
            return true;
        }

        /// <summary>
        /// 从索引移除
        /// </summary>
        public static bool Remove(EcsEntity root, int taskId)
        {
            var list = root.GetComponent<TaskListComponent>();
            if (!list.Id2Entities.TryGetValue(taskId, out var task)) return false;
            list.Id2Entities.Remove(taskId);

            if (list.ConfigId2Entities.TryGetValue(task.ConfigId, out var arr))
            {
                arr.Remove(task);
                if (arr.Count == 0) list.ConfigId2Entities.Remove(task.ConfigId);
            }

            if (list.AchieveItemId2TaskIds.TryGetValue(task.AchieveItemId, out var ids))
            {
                ids.Remove(taskId);
                if (ids.Count == 0) list.AchieveItemId2TaskIds.Remove(task.AchieveItemId);
            }
            return true;
        }

    public static TaskItem? Get(EcsEntity root, int taskId)
        {
            var list = root.GetComponent<TaskListComponent>();
            return list.Id2Entities.TryGetValue(taskId, out var task) ? task : null;
        }

        public static IReadOnlyList<TaskItem> GetByAchieveItem(EcsEntity root, int achieveItemId)
        {
            var list = root.GetComponent<TaskListComponent>();
            if (!list.AchieveItemId2TaskIds.TryGetValue(achieveItemId, out var ids) || ids.Count == 0)
                return Array.Empty<TaskItem>();
            var result = new List<TaskItem>(ids.Count);
            foreach (var id in ids)
            {
                if (list.Id2Entities.TryGetValue(id, out var task)) result.Add(task);
            }
            return result;
        }

        public static IReadOnlyList<TaskItem> GetAll(EcsEntity root)
        {
            var list = root.GetComponent<TaskListComponent>();
            return new List<TaskItem>(list.Id2Entities.Values);
        }

        /// <summary>
        /// 由 Achieve 的进度变化驱动任务进度与状态
        /// </summary>
        public static void OnAchieveProgressChanged(EcsEntity root, int achieveItemId, float progress, bool completed)
        {
            var list = root.GetComponent<TaskListComponent>();
            if (!list.AchieveItemId2TaskIds.TryGetValue(achieveItemId, out var ids) || ids.Count == 0) return;
            foreach (var id in ids)
            {
                if (!list.Id2Entities.TryGetValue(id, out var task)) continue;
                task.Progress = progress;
                task.Dispatch<IOnTaskProgressChanged>(s => s.OnTaskProgressChanged(task, task.TaskId, progress));
                var newState = TaskItemSystem.EvaluateState(task, completed);
                if (newState != task.State)
                {
                    task.State = newState;
                    if (newState == TaskState.Claimable)
                    {
                        task.CompletedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        task.Dispatch<IOnTaskCompleted>(s => s.OnTaskCompleted(task, task.TaskId));
                    }
                }
            }
        }

        public static bool Activate(EcsEntity root, int taskId)
        {
            var task = Get(root, taskId);
            if (task == null) return false;
            TaskItemSystem.Activate(task);
            return true;
        }

        public static bool Claim(EcsEntity root, int taskId, Func<int, bool> tryGrantReward)
        {
            var task = Get(root, taskId);
            if (task == null) return false;
            return TaskItemSystem.Claim(task, tryGrantReward);
        }

        public static bool Reset(EcsEntity root, int taskId, bool resetToInactive = true)
        {
            var task = Get(root, taskId);
            if (task == null) return false;
            TaskItemSystem.Reset(task, resetToInactive);
            return true;
        }

        /// <summary>
        /// 查询便捷方法：按文档对外暴露
        /// </summary>
        public static TaskItem? GetTask(EcsEntity root, int taskId) => Get(root, taskId);

        public static float GetProgress(EcsEntity root, int taskId)
        {
            var task = Get(root, taskId);
            return task != null ? TaskItemSystem.GetProgress(task) : 0f;
        }

        public static TaskState GetState(EcsEntity root, int taskId)
        {
            var task = Get(root, taskId);
            return task != null ? TaskItemSystem.GetState(task) : TaskState.Inactive;
        }

        public static IReadOnlyList<TaskItem> ListTasks(EcsEntity root) => GetAll(root);
    }
}
