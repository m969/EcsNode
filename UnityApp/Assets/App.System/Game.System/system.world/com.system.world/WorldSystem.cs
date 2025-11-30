using cfg.data;
using ECS;
using ECSGame.Module.GridBased;
using ECSGame.Module.Building;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TrueSync;
using ECSGame.UnitDispatchModule;
using UnityEngine;

namespace ECSGame
{
    public class WorldSystem : AEntitySystem<World>,
        IInit<World>,
        IUpdate<World>,
        IEventDispatch<ActorDeathEvent>
    {
        public static World Create(Assembly systemAssembly)
        {
            var world = EcsNodeSystem.Create<World>(EcsType.World, systemAssembly);
            world.AddComponent<ActorListComponent>();
            world.AddComponent<GridPlaneListComponent>();
            world.AddComponent<UnitDispatcherListComponent>();
            return world;
        }

        public void Init(World world)
        {
            // 创建建筑网格区域
            var config = GridPlaneConfig.GetOrDefault(1001);
            var gridPlane = GridPlaneSystem.CreateGridPlane(world, GridPlaneConfigWrap.Create(config), (0, 0));
            gridPlane.AddComponent<TransformComponent>();
            gridPlane.AddComponent<GridPlaneSelectionComponent>();
            TransformSystem.ChangePosition(gridPlane, new TSVector(0, 0, 0));
            TransformSystem.ChangeRotation(gridPlane, TSQuaternion.Euler(90, 0, 0));
            GridPlaneListSystem.AddGridPlane(world, gridPlane);
            gridPlane.Init();

            // 创建战场网格区域
            config = GridPlaneConfig.GetOrDefault(1002);
            gridPlane = GridPlaneSystem.CreateGridPlane(world, GridPlaneConfigWrap.Create(config), (0, -20));
            gridPlane.AddComponent<TransformComponent>();
            gridPlane.AddComponent<GridPlaneSelectionComponent>();
            TransformSystem.ChangePosition(gridPlane, new TSVector(0, 0, -20));
            TransformSystem.ChangeRotation(gridPlane, TSQuaternion.Euler(90, 0, 0));
            GridPlaneListSystem.AddGridPlane(world, gridPlane);
            gridPlane.Init();

            // 创建怪物单位派遣实体
            var monsterDispatcher = UnitDispatcherSystem.Create(world, 1001);
            monsterDispatcher.Timeout = Time.time + 5f; // 5秒后超时
            monsterDispatcher.AddComponent<DispatchRuleComponent>();
            monsterDispatcher.AddComponent<DispatchStateComponent>();
            UnitDispatcherListSystem.AddDispatcher(world, monsterDispatcher);
            UnitDispatcherSystem.StartDispatch(monsterDispatcher, 1);
            AppStatic.MonsterDispatcher = monsterDispatcher;
        }

        public void Update(World entity)
        {
        }

        public void OnHandleEvent(EcsNode ecsNode, ActorDeathEvent eventContext)
        {
            var world = ecsNode.As<World>();
            var actor = eventContext.Actor;
            if (actor == AppStatic.OtherActor)
            {
                //派遣新的怪物
                UnitDispatcherSystem.StartDispatch(AppStatic.MonsterDispatcher, 1);
            }
        }
    }
}
