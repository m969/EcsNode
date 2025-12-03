using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.ResourceDataModule
{
    public class ResourceDataSystem : AComponentSystem<EcsEntity, ResourceDataComponent>
    {
        /// <summary>
        /// 增加资源：内部执行合法性校验、日志写入、同步
        /// </summary>
        public static void GainResource(EcsEntity entity, int type, int value, long itemId = 0)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            // 初始化缺失的键
            if (!comp.ResourceValues.ContainsKey(type))
                comp.ResourceValues[type] = 0;

            comp.ResourceValues[type] += value;

            var newValue = comp.ResourceValues[type];
            var reason = BuildReason(itemId);

            // 写日志（正向变更）
            ResourceChangeLogSystem.AddChangeLog(entity, ResourceChangeType.Gain, value, reason, type, entity.Id);

            // 事件派发（资源变更）
            entity.Dispatch<IResourceChanged>(d => d.OnResourceChanged(entity, type, value, newValue, ResourceChangeType.Gain, reason));

            // 同步
            SyncResource(entity);
        }

        /// <summary>
        /// 消耗资源：不足返回false；成功则写日志与同步
        /// </summary>
        public static bool ConsumeResource(EcsEntity entity, int type, int value, long itemId = 0)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (!comp.ResourceValues.ContainsKey(type) || comp.ResourceValues[type] < value)
                return false;

            comp.ResourceValues[type] -= value;

            var newValue = comp.ResourceValues[type];
            var reason = BuildReason(itemId);

            // 写日志（负向变更用负值体现delta）
            ResourceChangeLogSystem.AddChangeLog(entity, ResourceChangeType.Consume, -value, reason, type, entity.Id);

            // 事件派发（资源变更，delta为负）
            entity.Dispatch<IResourceChanged>(d => d.OnResourceChanged(entity, type, -value, newValue, ResourceChangeType.Consume, reason));

            // 同步
            SyncResource(entity);
            return true;
        }

        /// <summary>
        /// 同步资源数据
        /// </summary>
        public static void SyncResource(EcsEntity entity)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            comp.LastSyncTime = DateTime.UtcNow;

            // 派发同步完成事件
            entity.Dispatch<IResourceSynced>(d => d.OnResourceSynced(entity, comp.LastSyncTime));
        }

        /// <summary>
        /// 校验资源合法性
        /// </summary>
        public static bool ValidateResource(EcsEntity entity, int type, int value)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            return value >= 0 && comp.ResourceValues.ContainsKey(type) && comp.ResourceValues[type] >= value;
        }

        /// <summary>
        /// 查询资源数值
        /// </summary>
        public static int GetResourceValue(EcsEntity entity, int type)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            return comp.ResourceValues.TryGetValue(type, out var v) ? v : 0;
        }

        /// <summary>
        /// 批量变更资源：原子预检，任一变更导致负值则整体取消
        /// 变更成功后逐条写日志与同步
        /// </summary>
        public static bool BatchChange(EcsEntity entity, Dictionary<int, int> changes)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (changes == null || changes.Count == 0) return true;

            // 快照预检
            var snapshot = new Dictionary<int, int>(comp.ResourceValues);
            foreach (var kv in changes)
            {
                if (!snapshot.ContainsKey(kv.Key)) snapshot[kv.Key] = 0;
                var next = snapshot[kv.Key] + kv.Value;
                if (next < 0) return false;
                snapshot[kv.Key] = next;
            }

            // 应用
            foreach (var kv in changes)
            {
                if (!comp.ResourceValues.ContainsKey(kv.Key)) comp.ResourceValues[kv.Key] = 0;
                comp.ResourceValues[kv.Key] += kv.Value;

                var changeType = kv.Value >= 0 ? ResourceChangeType.Gain : ResourceChangeType.Consume;
                ResourceChangeLogSystem.AddChangeLog(entity, changeType, kv.Value, string.Empty, kv.Key, entity.Id);
                var newValue = comp.ResourceValues[kv.Key];
                entity.Dispatch<IResourceChanged>(d => d.OnResourceChanged(entity, kv.Key, kv.Value, newValue, changeType, string.Empty));
            }

            SyncResource(entity);
            return true;
        }

        /// <summary>
        /// 重置资源数据
        /// </summary>
        public static void ResetResource(EcsEntity entity)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp.ResourceValues.Count > 0)
            {
                // 逐条派发清零事件（delta为负）
                var snapshot = new Dictionary<int, int>(comp.ResourceValues);
                foreach (var kv in snapshot)
                {
                    if (kv.Value != 0)
                    {
                        entity.Dispatch<IResourceChanged>(d => d.OnResourceChanged(entity, kv.Key, -kv.Value, 0, ResourceChangeType.Deduct, "reset"));
                    }
                }
                comp.ResourceValues.Clear();
            }
            SyncResource(entity);
        }

        /// <summary>
        /// 持久化资源数据（预留）
        /// </summary>
        public static void PersistResource(EcsEntity entity)
        {
            // 预留：通过事件或外部系统保存
            // 完成后派发同步（视为一次持久化写回完成）
            entity.Dispatch<IResourceSynced>(d => d.OnResourceSynced(entity, DateTime.UtcNow));
        }

        private static string BuildReason(long itemId)
        {
            return itemId > 0 ? $"item:{itemId}" : string.Empty;
        }
    }
}
