using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve
{
    /// <summary>
    /// 达成项系统
    /// </summary>
    public class AchieveItemSystem : AEntitySystem<AchieveItem>, IAwake<AchieveItem>, IInit<AchieveItem>
    {
        void IAwake<AchieveItem>.Awake(AchieveItem entity)
        {
            // 在实体被创建时调用
            entity.Status = AchieveStatus.NotStarted;
            entity.CreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.CompleteTime = 0;
        }

        void IInit<AchieveItem>.Init(AchieveItem entity)
        {
            // 在实体初始化时调用
        }

        /// <summary>
        /// 创建达成项实体
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="config">达成项配置</param>
        /// <returns>达成项实体</returns>
        public static AchieveItem Create(EcsEntity parent, IAchieveItemConfig config)
        {
            var entity = parent.AddChild<AchieveItem>(e =>
            {
                e.Name = config.Name;
                e.Description = config.Description;
                e.Type = config.Type;
                e.Status = AchieveStatus.NotStarted;
                e.CreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                e.CompleteTime = 0;
                e.Priority = config.Priority;
            });

            entity.AddComponent<AchieveItemConditionComponent>();
            entity.AddComponent<AchieveItemRewardComponent>();

            return entity;
        }

        /// <summary>
        /// 初始化达成项实体
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <param name="config">达成项配置</param>
        public static void Init(AchieveItem entity, IAchieveItemConfig config)
        {
            entity.Name = config.Name;
            entity.Description = config.Description;
            entity.Type = config.Type;
            entity.Status = AchieveStatus.NotStarted;
            entity.CreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.CompleteTime = 0;
            entity.Priority = config.Priority;
        }

        /// <summary>
        /// 更新达成项状态
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <param name="status">目标状态</param>
        public static void UpdateStatus(AchieveItem entity, AchieveStatus status)
        {
            entity.Status = status;
            if (status == AchieveStatus.Completed)
            {
                entity.CompleteTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// 判定达成项是否完成
        /// </summary>
        /// <param name="entity">达成项实体</param>
        /// <returns>是否完成</returns>
        public static bool IsCompleted(AchieveItem entity)
        {
            return entity.Status == AchieveStatus.Completed;
        }
    }
}
