using ECS;
using UnityEngine;
using System;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 资源消耗系统，校验并扣除建造、升级所需资源。
    /// 生命周期接口为成员函数，其余接口为静态方法。
    /// </summary>
    public class BuildingCostSystem : AComponentSystem<BuildingEntity, BuildingCostComponent>
    {
        public override void Awake(BuildingEntity entity, BuildingCostComponent component) { }
        public override void Init(BuildingEntity entity, BuildingCostComponent component) { }
        public override void AfterInit(BuildingEntity entity, BuildingCostComponent component) { }
        public override void Enable(BuildingEntity entity, BuildingCostComponent component) { }
        public override void Disable(BuildingEntity entity, BuildingCostComponent component) { }
        public override void Destroy(BuildingEntity entity, BuildingCostComponent component) { }

        /// <summary>
        /// 校验建筑建造或升级所需资源是否充足。
        /// </summary>
        public static bool CheckCost(BuildingEntity entity)
        {
            var costComponent = entity.GetComponent<BuildingCostComponent>();
            var player = entity.Parent as PlayerEntity;
            if (player == null) return false;
            var resourceComponent = player.GetComponent<ResourceComponent>();
            if (resourceComponent == null) return false;

            foreach (var kv in costComponent.CostDict)
            {
                if (!resourceComponent.ResourceDict.TryGetValue(kv.Key, out var value) || value < kv.Value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 扣除建筑建造或升级所需资源。
        /// </summary>
        public static void DeductCost(BuildingEntity entity)
        {
            var costComponent = entity.GetComponent<BuildingCostComponent>();
            var player = entity.Parent as PlayerEntity;
            if (player == null) return;
            var resourceComponent = player.GetComponent<ResourceComponent>();
            if (resourceComponent == null) return;

            foreach (var kv in costComponent.CostDict)
            {
                if (resourceComponent.ResourceDict.ContainsKey(kv.Key))
                    resourceComponent.ResourceDict[kv.Key] -= kv.Value;
            }
        }
    }
}
