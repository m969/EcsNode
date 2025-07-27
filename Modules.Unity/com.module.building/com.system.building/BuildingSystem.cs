using ECS;
using UnityEngine;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑系统，作为BuildingEntity的通用系统入口，负责调度和管理建筑相关的通用逻辑。
    /// 生命周期接口为成员函数，其余接口为静态方法。
    /// </summary>
    public class BuildingSystem : AEntitySystem<BuildingEntity>
    {
        public override void Awake(BuildingEntity entity) { /* ... */ }
        public override void Init(BuildingEntity entity) { /* ... */ }
        public override void AfterInit(BuildingEntity entity) { /* ... */ }
        public override void Enable(BuildingEntity entity) { /* ... */ }
        public override void Disable(BuildingEntity entity) { /* ... */ }
        public override void Update(BuildingEntity entity) { /* ... */ }
        public override void Destroy(BuildingEntity entity) { /* ... */ }
    }
}

