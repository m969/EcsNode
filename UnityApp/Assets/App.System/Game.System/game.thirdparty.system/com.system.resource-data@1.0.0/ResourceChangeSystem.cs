using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.ResourceData
{
    public class ResourceChangeSystem : AComponentSystem<EcsEntity, ResourceChangeComponent>
    {
        public void Awake(EcsEntity entity, ResourceChangeComponent component) { }
        public void Init(EcsEntity entity, ResourceChangeComponent component) { }
        public void AfterInit(EcsEntity entity, ResourceChangeComponent component) { }
        public void Enable(EcsEntity entity, ResourceChangeComponent component) { }
        public void Disable(EcsEntity entity, ResourceChangeComponent component) { }
        public void Destroy(EcsEntity entity, ResourceChangeComponent component) { }

        /// <summary>
        /// 处理单次资源变更
        /// </summary>
        public static void HandleChange(EcsEntity entity, ResourceChangeType changeType, int value, string reason, ResourceType type)
        {
            var comp = entity.GetComponent<ResourceChangeComponent>();
            if (comp == null) return;
            comp.ChangeType = changeType;
            comp.ChangeValue = value;
            comp.Reason = reason;
            comp.ResourceType = type;
            comp.ChangeTime = DateTime.Now;
            // 可扩展：触发日志、事件等
        }

        /// <summary>
        /// 校验资源变更合法性
        /// </summary>
        public static bool ValidateChange(EcsEntity entity, ResourceChangeType changeType, int value, ResourceType type)
        {
            // 可扩展：根据业务规则校验
            return value >= 0;
        }

        /// <summary>
        /// 触发资源变更相关事件
        /// </summary>
        public static void TriggerChangeEvent(EcsEntity entity, ResourceChangeType changeType, int value, ResourceType type)
        {
            // 可扩展：分发事件
        }
    }
}
