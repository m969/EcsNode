using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.ResourceDataModule
{
    public class ResourceChangeLogSystem : AEntitySystem<ResourceChangeLog>
    {
        /// <summary>
        /// 记录资源变更日志（在owner下创建子实体）
        /// </summary>
        /// <param name="owner">日志所属实体（例如玩家或其资源实体）</param>
        /// <param name="changeType">变更类型</param>
        /// <param name="value">变更数值，正为增加，负为减少</param>
        /// <param name="reason">变更原因</param>
        /// <param name="type">资源类型</param>
        /// <param name="ownerId">所属玩家Id</param>
        public static ResourceChangeLog AddChangeLog(EcsEntity owner, ResourceChangeType changeType, int value, string reason, int type, long ownerId)
        {
            return owner.AddChild<ResourceChangeLog>(log =>
            {
                log.ChangeType = changeType;
                log.ChangeValue = value;
                log.Reason = reason;
                log.ResourceType = type;
                log.OwnerId = ownerId;
                log.ChangeTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// 查询资源变更记录
        /// </summary>
        /// <param name="owner">日志所属实体</param>
        /// <param name="filter">筛选谓词，可为空</param>
        public static List<ResourceChangeLog> QueryChangeLogs(EcsEntity owner, Func<ResourceChangeLog, bool> filter)
        {
            var result = new List<ResourceChangeLog>();
            foreach (var kv in owner.Id2Children)
            {
                if (kv.Value is ResourceChangeLog log)
                {
                    if (filter == null || filter(log))
                        result.Add(log);
                }
            }
            return result;
        }

        /// <summary>
        /// 统计资源变更数据
        /// </summary>
        /// <param name="owner">日志所属实体</param>
        /// <param name="type">资源类型（可选，传入null统计所有类型）</param>
        public static int StatChangeLogs(EcsEntity owner, int type)
        {
            int sum = 0;
            foreach (var kv in owner.Id2Children)
            {
                if (kv.Value is ResourceChangeLog log)
                {
                    if (log.ResourceType == type)
                        sum += log.ChangeValue;
                }
            }
            return sum;
        }
    }
}
