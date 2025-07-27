using ECS;
using UnityEngine;
using ECSGame.Module.Building;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 建筑效果系统，根据建筑属性动态调整资源产出、人口上限等。
    /// 生命周期接口为成员函数。
    /// </summary>
    public class BuildingEffectSystem : AComponentSystem<BuildingEntity, BuildingEffectComponent>
    {
        public override void Awake(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 初始化建筑效果组件
            // 可根据建筑类型设置初始效果
            component.IsActive = false;
            component.ResourceOutput = 0;
            component.PopulationLimit = 0;
        }

        public override void Init(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 根据建筑属性初始化资源产出和人口上限
            var data = entity.BuildingData;
            if (data != null)
            {
                component.ResourceOutput = data.BaseResourceOutput;
                component.PopulationLimit = data.BasePopulationLimit;
            }
        }

        public override void AfterInit(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 可用于根据外部条件调整效果
            if (entity.Level > 1)
            {
                component.ResourceOutput += entity.Level * 10;
                component.PopulationLimit += entity.Level * 5;
            }
        }

        public override void Enable(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 激活建筑效果
            component.IsActive = true;
        }

        public override void Disable(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 禁用建筑效果
            component.IsActive = false;
        }

        public override void Destroy(BuildingEntity entity, BuildingEffectComponent component)
        {
            // 清理建筑效果组件
            component.IsActive = false;
            component.ResourceOutput = 0;
            component.PopulationLimit = 0;
        }
    }
}
