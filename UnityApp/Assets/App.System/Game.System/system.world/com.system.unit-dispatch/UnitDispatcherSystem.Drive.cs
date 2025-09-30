using ECS;
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

        public void OnDispatchStarted(EcsEntity entity, int count)
        {
            ConsoleLog.Debug($"OnDispatchStarted: EntityId={entity.Id}");
            var dispatcher = entity.As<UnitDispatcher>();
            if (dispatcher.ConfigId == 1001)
            {
                var world = entity.GetParent<World>();

                var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1002);
                var gridCell = GridCellListSystem.GetCell(gridPlane, 1, 1);
                var gridCellPos = new TrueSync.TSVector(gridCell.X, 0, gridCell.Y) + TransformSystem.GetPosition(gridPlane);

                var actor = ActorSystem.Create(world, world.NewEntityId());
                actor.Type = ActorType.Monster;
                ActorListSystem.AddActor(world, actor);
                TransformSystem.ChangePosition(actor, gridCellPos);
                CollisionSystem.SetLayer(actor, 1);
                actor.Init();
                AISystem.CreateNode<MoveInputAIAction>(AIBehaviourType.Caution, actor);
            }

            if (dispatcher.ConfigId == 1002)
            {
                var building = entity.GetParent<BuildingEntity>();
                var world = building.GetParent<World>();

                var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1001);
                var gridCell = GridCellListSystem.GetCell(gridPlane, 1, 1);
                var gridCellPos = new TrueSync.TSVector(gridCell.X, 0, gridCell.Y) + TransformSystem.GetPosition(gridPlane);

                var actor = ActorSystem.Create(world, world.NewEntityId());
                actor.Type = ActorType.Hero;
                ActorListSystem.AddActor(world, actor);
                TransformSystem.ChangePosition(actor, gridCellPos);
                CollisionSystem.SetLayer(actor, 1);
                //TaskItemSystem.Create(actor, );
                actor.Init();
                AISystem.CreateNode<MoveInputAIAction>(AIBehaviourType.Launch, actor);
            }
        }

        public void OnDispatchCompleted(EcsEntity entity)
        {
            ConsoleLog.Debug($"OnDispatchCompleted: EntityId={entity.Id}");
        }
    }
}
