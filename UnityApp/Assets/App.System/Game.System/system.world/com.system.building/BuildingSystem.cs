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

namespace ECSGame.Module.Building
{
    public partial class BuildingSystem : AEntitySystem<BuildingEntity>,
        IUpdate<BuildingEntity>,
        IOnDispatchTimeout
    {
        public void Update(BuildingEntity entity)
        {
            DispatchAgentSystem.Tick(entity, AppStatic.DeltaTimeSeconds);
        }

        public void OnDispatchTimeout(EcsEntity entity)
        {
            ConsoleLog.Debug($"OnDispatchTimeout: EntityId={entity.Id}");

            var dispatcher = entity.GetComponent<DispatchAgentComponent>();
            if (dispatcher.ConfigId == 1002)
            {
                var building = dispatcher.Entity.As<BuildingEntity>();
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
                actor.Init();
                MoveSystem.ChangeSpeed(actor, 1);
                AppStatic.MyActor = actor;
                AISystem.StartBehaviour<AIBehaviour_Launch>(actor);
            }
        }
    }
}
