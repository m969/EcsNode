using ECS;
using ECSGame.ChaseModule;
using ECSGame.Module.Building;
using ECSGame.Module.GridBased;
using ECSGame.TaskModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ECSGame.UnitDispatchModule
{
    public partial class UnitDispatcherSystem :
        IUpdate<UnitDispatcher>,
        IOnDispatchStarted,
        IOnDispatchTimeout,
        IOnDispatchCompleted
    {
        public void Update(UnitDispatcher entity)
        {
            if (entity.DispatchCount <= 0)
            {
                return;
            }
            DispatchStateSystem.Tick(entity, UnityEngine.Time.deltaTime);
        }

        public void OnDispatchTimeout(EcsEntity entity)
        {
            ConsoleLog.Debug($"OnDispatchTimeout: EntityId={entity.Id}");
        }

        /// <summary>
        /// 派遣开始时触发
        /// </summary>
        public void OnDispatchStarted(EcsEntity entity, int count)
        {
            ConsoleLog.Debug($"OnDispatchStarted: EntityId={entity.Id}");
            var dispatcher = entity.As<UnitDispatcher>();
            if (dispatcher.ConfigId == 1001)
            {
                var world = dispatcher.GetParent<World>();

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

            if (dispatcher.ConfigId == 1002)
            {
                var building = dispatcher.GetParent<BuildingEntity>();
                var world = building.GetParent<World>();

                var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1001);
                var gridCell = GridCellListSystem.GetCell(gridPlane, 1, 1);
                var gridCellPos = new TrueSync.TSVector(gridCell.X, 0, gridCell.Y) + TransformSystem.GetPosition(gridPlane);

                var actor = ActorSystem.Create(world, world.NewEntityId());
                actor.Type = ActorType.Hero;
                ActorListSystem.AddActor(world, actor);
                TransformSystem.ChangePosition(actor, gridCellPos);
                CollisionSystem.SetLayer(actor, 1);
                ChaseConfigSystem.SetConfig(actor, "", 50, 51, 2, 55);
                ChaseSystem.SetCurrentTarget(actor, AppStatic.OtherActor);
                // ConsoleLog.Debug($"Set Hero's chase target to OtherActor Id={AppStatic.OtherActor.Id} {actor.GetComponent<ChaseComponent>().CurrentTargetId}");
                //TaskItemSystem.Create(actor, );
                actor.Init();
                MoveSystem.ChangeSpeed(actor, 1);
                AppStatic.MyActor = actor;
                AISystem.StartBehaviour<AIBehaviour_Launch>(actor);
            }
        }

        public void OnDispatchCompleted(EcsEntity entity)
        {
            ConsoleLog.Debug($"OnDispatchCompleted: EntityId={entity.Id}");
        }
    }
}
