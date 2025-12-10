using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.AttackModule
{
    // 组件系统：管理 AttackListComponent 的数据操作
    public class AttackListSystem : AComponentSystem<EcsEntity, AttackListComponent>
    {
        /// <summary>
        /// 创建 AttackAction 实体并注册到列表
        /// </summary>
        public static AttackAction Create(EcsEntity actor, in AttackContext ctx, IAttackConfig cfg)
        {
            var list = actor.GetComponent<AttackListComponent>();
            var ctxCopy = ctx;
            var action = actor.AddChild<AttackAction>(e =>
            {
                e.AttackerId = ctxCopy.AttackerId;
                e.TargetId = ctxCopy.TargetId;
                e.ConfigId = ctxCopy.ConfigId;
                e.StartTimeMs = ctxCopy.StartTimeMs;
            });

            list.Id2Entities[action.Id] = action;

            if (!list.ConfigId2Entities.TryGetValue(cfg.Id, out var bucket))
            {
                bucket = new List<AttackAction>();
                list.ConfigId2Entities[cfg.Id] = bucket;
            }
            bucket.Add(action);

            list.AttackerId2LatestAttackId[ctx.AttackerId] = action.Id;

            return action!;
        }

        public static void Tick(EcsEntity actor, long nowMs)
        {
            var list = actor.GetComponent<AttackListComponent>();
            if (list == null || list.Id2Entities.Count == 0) return;

            // 收集以避免遍历时修改集合
            AttackListComponent.CachedRemoveList.Clear();
            foreach (var kv in list.Id2Entities)
            {
                var action = kv.Value;
                AttackActionSystem.Tick(action, nowMs);
                var tl = action.GetComponent<AttackTimelineComponent>();
                if (tl.CurrentPhase == AttackPhase.Ended || tl.CurrentPhase == AttackPhase.Canceled)
                {
                    // 收集后统一移除
                    AttackListComponent.CachedRemoveList.Add(action.Id);
                }
            }

            foreach (var id in AttackListComponent.CachedRemoveList)
            {
                AttackListSystem.Remove(actor, id);
            }
            AttackListComponent.CachedRemoveList.Clear();
        }

        /// <summary>
        /// 从列表移除 AttackAction
        /// </summary>
        public static bool Remove(EcsEntity actor, long actionId)
        {
            var list = actor.GetComponent<AttackListComponent>();
            if (!list.Id2Entities.TryGetValue(actionId, out var action))
                return false;

            list.Id2Entities.Remove(actionId);

            if (list.ConfigId2Entities.TryGetValue(action.ConfigId, out var bucket))
            {
                bucket.Remove(action);
                if (bucket.Count == 0)
                {
                    list.ConfigId2Entities.Remove(action.ConfigId);
                }
            }

            if (list.AttackerId2LatestAttackId.TryGetValue(action.AttackerId, out var latest) && latest == actionId)
            {
                list.AttackerId2LatestAttackId.Remove(action.AttackerId);
            }

            // 实体生命周期的销毁交由外层ECS管理，这里仅解除索引注册
            return true;
        }

        /// <summary>
        /// 根据 Id 获取 AttackAction
        /// </summary>
        public static AttackAction Get(EcsEntity actor, long actionId)
        {
            var list = actor.GetComponent<AttackListComponent>();
            list.Id2Entities.TryGetValue(actionId, out var action);
            return action!;
        }

        /// <summary>
        /// 按配置分组获取
        /// </summary>
        public static IReadOnlyList<AttackAction> GetByConfig(EcsEntity actor, int configId)
        {
            var list = actor.GetComponent<AttackListComponent>();
            if (list.ConfigId2Entities.TryGetValue(configId, out var bucket))
            {
                return bucket;
            }
            return Array.Empty<AttackAction>();
        }

        /// <summary>
        /// 遍历所有 AttackAction
        /// </summary>
        public static void ForEach(EcsEntity actor, Action<AttackAction> visitor)
        {
            var list = actor.GetComponent<AttackListComponent>();
            foreach (var kv in list.Id2Entities)
            {
                visitor?.Invoke(kv.Value);
            }
        }
    }
}
