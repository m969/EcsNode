using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.ResourceData
{
    public class ResourceChangeLogSystem : AEntitySystem<ResourceChangeLog>
    {
        public void Awake(ResourceChangeLog entity) { }
        public void Init(ResourceChangeLog entity) { }
        public void AfterInit(ResourceChangeLog entity) { }
        public void Enable(ResourceChangeLog entity) { }
        public void Disable(ResourceChangeLog entity) { }
        public void Update(ResourceChangeLog entity) { }
        public void Destroy(ResourceChangeLog entity) { }

        /// <summary>
        /// 记录资源变更日志
        /// </summary>
        public static void AddChangeLog(ResourceChangeLog entity, ResourceChangeType changeType, int value, string reason, ResourceType type, long ownerId)
        {
            entity.ChangeType = changeType;
            entity.ChangeValue = value;
            entity.Reason = reason;
            entity.ResourceType = type;
            entity.OwnerId = ownerId;
            entity.ChangeTime = DateTime.Now;
            // 可扩展：添加到日志列表
        }

        /// <summary>
        /// 查询资源变更记录
        /// </summary>
        public static List<ResourceChangeLog> QueryChangeLogs(ResourceChangeLog entity, Func<ResourceChangeLog, bool> filter)
        {
            // 可扩展：根据filter查询日志
            return new List<ResourceChangeLog>();
        }

        /// <summary>
        /// 统计资源变更数据
        /// </summary>
        public static int StatChangeLogs(ResourceChangeLog entity, ResourceType type)
        {
            // 可扩展：统计指定资源类型的变更总数
            return 0;
        }
    }
}
