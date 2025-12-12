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
using ECSGame.ResourceDataModule;
using ECSGame.ChaseModule;

namespace ECSGame
{
    public class WorldSystem : AEntitySystem<World>,
        IInit<World>,
        IUpdate<World>,
        IOnDispatchTimeout,
        IEventHandle<ActorDeathEvent>
    {
        public static World Create(Assembly systemAssembly)
        {
            var world = EcsNodeSystem.Create<World>(EcsType.World, systemAssembly);
            world.AddComponent<ActorListComponent>();
            world.AddComponent<GridPlaneListComponent>();
            world.AddComponent<DispatchAgentComponent>();
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
            // var monsterDispatcher = 
            // monsterDispatcher.Timeout = 5f; // 5秒后超时
            // monsterDispatcher.AddComponent<DispatchRuleComponent>();
            // monsterDispatcher.AddComponent<DispatchStateComponent>();
            // UnitDispatcherListSystem.AddDispatcher(world, monsterDispatcher);
            // UnitDispatcherSystem.StartDispatch(monsterDispatcher, 1);
            // AppStatic.MonsterDispatcher = monsterDispatcher;
            DispatchAgentSystem.StartDispatch(world, 1, 1, 5f);
        }

        public static BuildingEntity CreateBuildingFromGrid(World world)
        {
            var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1001);
            var selectedCellId = GridPlaneSelectionSystem.GetSelectCell(gridPlane);
            var gridCell = GridCellListSystem.GetCellById(gridPlane, selectedCellId);
            var position = new ECSGame.Module.Building.Vector2Int(gridCell.X, gridCell.Y);
            var building = BuildingSystem.Create(world, 1, position, EcsDomain.Player.Id);
            building.AddComponent<TransformComponent>();
            building.AddComponent<DispatchAgentComponent>();
            TransformSystem.ChangePosition(building, new TrueSync.TSVector(position.x, 0, position.y));
            building.Init();
            return building;
        }

        public void Update(World entity)
        {
            DispatchAgentSystem.Tick(entity, AppStatic.DeltaTimeSeconds);
        }

        public void OnDispatchTimeout(EcsEntity entity)
        {
            ConsoleLog.Debug($"OnDispatchTimeout: EntityId={entity.Id}");
            var dispatcher = entity.GetComponent<DispatchAgentComponent>();
            if (dispatcher.ConfigId == 1001)
            {
                var world = entity.As<World>();

                var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1002);
                var gridCell = GridCellListSystem.GetCell(gridPlane, 1, 1);
                var gridCellPos = new TrueSync.TSVector(gridCell.X, 0, gridCell.Y) + TransformSystem.GetPosition(gridPlane);

                var actor = ActorSystem.Create(world, world.NewEntityId());
                actor.Type = ActorType.Monster;
                ActorListSystem.AddActor(world, actor);
                TransformSystem.ChangePosition(actor, gridCellPos);
                ConsoleLog.Debug($"gridCellPos1={gridCellPos}");
                CollisionSystem.SetLayer(actor, 1);
                actor.Init();
                AppStatic.OtherActor = actor;

                gridCell = GridCellListSystem.GetCell(gridPlane, 1, 8);
                gridCellPos = new TrueSync.TSVector(gridCell.X, 0, gridCell.Y) + TransformSystem.GetPosition(gridPlane);
                ConsoleLog.Debug($"gridCellPos2={gridCellPos}");
                MoveSystem.ChangeDestination(actor, gridCellPos);
                MoveSystem.ChangeSpeed(actor, 1);
                AISystem.StartBehaviour<AIBehaviour_MoveToDestination>(actor);
            }
        }

        /// <summary>
        /// 处理角色死亡事件
        /// </summary>
        /// <param name="ecsNode"></param>
        /// <param name="eventContext"></param>
        public void OnHandleEvent(EcsNode ecsNode, ActorDeathEvent eventContext)
        {
            var world = ecsNode.As<World>();
            var actor = eventContext.Actor;
            if (actor == AppStatic.OtherActor)
            {
                //添加道具奖励给玩家
                ResourceDataSystem.GainResource(AppStatic.MyActor, ResourceType.Coin, 10);
                //派遣新的怪物
                // UnitDispatcherSystem.StartDispatch(AppStatic.MonsterDispatcher, 1);
                DispatchAgentSystem.StartDispatch(world, 1, 1, 5f);
            }
        }
    }
}
