using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Message
{
    /// <summary>
    /// Neter消息组件系统：
    /// - 负责生命周期初始化（分配字典）
    /// - 提供静态业务方法用于Id/Type映射查询与初始化挂载
    /// </summary>
    public class NeterMessageSystem : AComponentSystem<EcsEntity, NeterMessageComponent>,
        IAwake<EcsEntity, NeterMessageComponent>,
        IInit<EcsEntity, NeterMessageComponent>,
        IAfterInit<EcsEntity, NeterMessageComponent>,
        IEnable<EcsEntity, NeterMessageComponent>,
        IDisable<EcsEntity, NeterMessageComponent>,
        IDestroy<EcsEntity, NeterMessageComponent>
    {
        public void Awake(EcsEntity entity, NeterMessageComponent component)
        {
            component.Id2Type ??= new Dictionary<int, Type>();
            component.Type2Id ??= new Dictionary<Type, int>();
        }

        public void Init(EcsEntity entity, NeterMessageComponent component) { }
        public void AfterInit(EcsEntity entity, NeterMessageComponent component) { }
        public void Enable(EcsEntity entity, NeterMessageComponent component) { }
        public void Disable(EcsEntity entity, NeterMessageComponent component) { }
        public void Destroy(EcsEntity entity, NeterMessageComponent component) { }

        /// <summary>
        /// 获取消息Id对应的消息类型。
        /// 未找到返回 null。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="messageId">消息Id</param>
        public static Type GetMessageType(EcsEntity entity, int messageId)
        {
            var comp = entity.GetComponent<NeterMessageComponent>();
            return comp.Id2Type.TryGetValue(messageId, out var type) ? type : null;
        }

        /// <summary>
        /// 获取消息类型对应的消息Id。
        /// 未找到返回 -1。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="messageType">消息类型</param>
        public static int GetMessageTypeId(EcsEntity entity, Type messageType)
        {
            var comp = entity.GetComponent<NeterMessageComponent>();
            return comp.Type2Id.TryGetValue(messageType, out var id) ? id : -1;
        }

        private static void RegisterId2Messages(NeterMessageComponent comp, Dictionary<int, Type> id2Type)
        {
            comp.Id2Type = id2Type;
            comp.Type2Id.Clear();
            foreach (var kv in comp.Id2Type)
            {
                comp.Type2Id[kv.Value] = kv.Key;
            }
        }
    }
}
