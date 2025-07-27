using ECS;
using UnityEngine;
using System;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑建造系统，处理建筑的建造、升级、拆除流程，管理建造队列。
    /// 生命周期接口为成员函数，其余接口为静态方法。
    /// </summary>
    public class BuildingConstructionSystem : AEntitySystem<BuildingEntity>
    {
        public override void Awake(BuildingEntity entity)
        {
            // 初始化建造状态
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null)
            {
                state.ConstructionProgress = 0f;
                state.IsConstructed = false;
            }
        }

        public override void Init(BuildingEntity entity)
        {
            // 初始化建造消耗
            var cost = entity.GetComponent<BuildingCostComponent>();
            if (cost != null)
            {
                cost.CurrentCost = cost.BaseCost;
            }
        }

        public override void AfterInit(BuildingEntity entity)
        {
            // 可用于建造队列注册等逻辑
        }

        public override void Enable(BuildingEntity entity)
        {
            // 激活建造流程
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null)
            {
                state.IsActive = true;
            }
        }

        public override void Disable(BuildingEntity entity)
        {
            // 禁用建造流程
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null)
            {
                state.IsActive = false;
            }
        }

        public override void Update(BuildingEntity entity)
        {
            // 建造进度推进
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null && !state.IsConstructed && state.IsActive)
            {
                state.ConstructionProgress += Time.deltaTime;
                if (state.ConstructionProgress >= state.ConstructionTime)
                {
                    state.IsConstructed = true;
                    state.IsActive = false;
                }
            }
        }

        public override void Destroy(BuildingEntity entity)
        {
            // 清理建造相关组件
            entity.RemoveComponent<BuildingStateComponent>();
            entity.RemoveComponent<BuildingCostComponent>();
        }

        /// <summary>
        /// 发起建造，buildLevel为目标等级
        /// </summary>
        public static void StartBuild(BuildingEntity entity, int buildLevel)
        {
            // 获取组件，处理建造逻辑
            var state = entity.GetComponent<BuildingStateComponent>();
            var cost = entity.GetComponent<BuildingCostComponent>();
            if (state != null && cost != null)
            {
                state.IsActive = true;
                state.IsConstructed = false;
                state.ConstructionProgress = 0f;
                state.ConstructionTime = buildLevel * 10f; // 示例：每级10秒
                cost.CurrentCost = cost.BaseCost * buildLevel;
            }
        }

        /// <summary>
        /// 发起升级，targetLevel为目标等级
        /// </summary>
        public static void UpgradeBuild(BuildingEntity entity, int targetLevel)
        {
            // 获取组件，处理升级逻辑
            var state = entity.GetComponent<BuildingStateComponent>();
            var cost = entity.GetComponent<BuildingCostComponent>();
            if (state != null && cost != null && state.IsConstructed)
            {
                state.IsActive = true;
                state.IsConstructed = false;
                state.ConstructionProgress = 0f;
                state.ConstructionTime = targetLevel * 15f; // 示例：升级每级15秒
                cost.CurrentCost = cost.BaseCost * targetLevel;
            }
        }

        /// <summary>
        /// 取消建造/升级
        /// </summary>
        public static void CancelBuild(BuildingEntity entity)
        {
            // 获取组件，处理取消逻辑
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null && state.IsActive)
            {
                state.IsActive = false;
                state.ConstructionProgress = 0f;
            }
        }

        /// <summary>
        /// 拆除建筑
        /// </summary>
        public static void RemoveBuild(BuildingEntity entity)
        {
            // 获取组件，处理拆除逻辑
            var state = entity.GetComponent<BuildingStateComponent>();
            if (state != null)
            {
                state.IsActive = false;
                state.IsConstructed = false;
                state.ConstructionProgress = 0f;
            }
            entity.RemoveComponent<BuildingStateComponent>();
            entity.RemoveComponent<BuildingCostComponent>();
        }
    }
}

