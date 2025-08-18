using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.ResourceData
{
    public class ResourceDataSystem : AComponentSystem<EcsEntity, ResourceDataComponent>
    {
        public void Awake(EcsEntity entity, ResourceDataComponent component) { }
        public void Init(EcsEntity entity, ResourceDataComponent component) { }
        public void AfterInit(EcsEntity entity, ResourceDataComponent component) { }
        public void Enable(EcsEntity entity, ResourceDataComponent component) { }
        public void Disable(EcsEntity entity, ResourceDataComponent component) { }
        public void Destroy(EcsEntity entity, ResourceDataComponent component) { }

        /// <summary>
        /// 增加资源
        /// </summary>
        public static void GainResource(EcsEntity entity, ResourceType type, int value, long itemId = 0)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return;
            if (!comp.ResourceValues.ContainsKey(type))
                comp.ResourceValues[type] = 0;
            comp.ResourceValues[type] += value;
        }

        /// <summary>
        /// 消耗资源
        /// </summary>
        public static bool ConsumeResource(EcsEntity entity, ResourceType type, int value, long itemId = 0)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return false;
            if (!comp.ResourceValues.ContainsKey(type) || comp.ResourceValues[type] < value)
                return false;
            comp.ResourceValues[type] -= value;
            return true;
        }

        /// <summary>
        /// 同步资源数据
        /// </summary>
        public static void SyncResource(EcsEntity entity)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return;
            comp.LastSyncTime = DateTime.Now;
            // 可扩展：同步到服务器或客户端
        }

        /// <summary>
        /// 校验资源合法性
        /// </summary>
        public static bool ValidateResource(EcsEntity entity, ResourceType type, int value)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return false;
            return comp.ResourceValues.ContainsKey(type) && comp.ResourceValues[type] >= value;
        }

        /// <summary>
        /// 查询资源数值
        /// </summary>
        public static int GetResourceValue(EcsEntity entity, ResourceType type)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return 0;
            return comp.ResourceValues.TryGetValue(type, out var v) ? v : 0;
        }

        /// <summary>
        /// 批量变更资源
        /// </summary>
        public static void BatchChange(EcsEntity entity, Dictionary<ResourceType, int> changes)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return;
            foreach (var kv in changes)
            {
                if (!comp.ResourceValues.ContainsKey(kv.Key))
                    comp.ResourceValues[kv.Key] = 0;
                comp.ResourceValues[kv.Key] += kv.Value;
            }
        }

        /// <summary>
        /// 重置资源数据
        /// </summary>
        public static void ResetResource(EcsEntity entity)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return;
            comp.ResourceValues.Clear();
        }

        /// <summary>
        /// 持久化资源数据
        /// </summary>
        public static void PersistResource(EcsEntity entity)
        {
            var comp = entity.GetComponent<ResourceDataComponent>();
            if (comp == null) return;
            // 可扩展：保存到数据库或本地
        }
    }
}
