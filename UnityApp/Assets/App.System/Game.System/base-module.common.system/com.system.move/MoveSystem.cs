using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;

namespace ECSGame
{
    public partial class MoveSystem : AComponentSystem<EcsEntity, MoveComponent>,
        IAwake<EcsEntity, MoveComponent>
    {
        public void Awake(EcsEntity actor, MoveComponent moveComponent)
        {
        }

        public static void ChangeSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Speed = value;
        }

        public static void ChangeStopSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.StopSpeed = value;
        }

        public static void ChangeDirection(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.TrueDirection = target;
        }

        public static void ChangeDestination(EcsEntity entity, TSVector destination)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Destination = destination;
        }

        public static TSVector GetDestination(EcsEntity entity)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            return moveComp.Destination;
        }

        public static void StartMove(EcsEntity entity)
        {
            // ConsoleLog.Debug($"StartMove: EntityId={entity.Id}");
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Moving = true;
        }

        public static void StopMove(EcsEntity entity)
        {
            // ConsoleLog.Debug($"StopMove: EntityId={entity.Id}");
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Moving = false;
        }

        public static void Update(EcsEntity entity, MoveComponent moveComp, TSVector target)
        {
            if (!moveComp.Moving)
            {
                return;
            }
            var transComp = entity.GetComponent<TransformComponent>();
            moveComp.TrueDirection = target;
            var beforePos = transComp.Position;
            var afterPos = transComp.Position + target * FP.FromFloat(moveComp.Speed * AppStatic.DeltaTimeSeconds);
            TransformSystem.ChangePosition(entity, afterPos);
            if (TSVector.Distance(afterPos, target) < FP.FromFloat(0.1f))
            {
                StopMove(entity);
            }
        }
    }
}
