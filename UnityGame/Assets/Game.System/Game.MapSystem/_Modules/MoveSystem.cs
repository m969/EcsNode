using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;
using static UnityEngine.EventSystems.EventTrigger;

namespace ECSGame
{
    public class MoveSystem : AComponentSystem<EcsEntity, MoveComponent>,
IAwake<EcsEntity, MoveComponent>
    {
        public void Awake(EcsEntity actor, MoveComponent moveComponent)
        {
        }

        public static void SetSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Speed = value;
        }

        public static void SetStopSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.StopSpeed = value;
        }

        public static void ChangeMove(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.TrueDirection = target;
        }

        public const float SpeedAdaptive = 0.01f;

        public static FramePlay_Move MoveFrame(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            moveComp.TrueDirection = target;
            var beforePos = transComp.Position;
            var afterPos = transComp.Position + target * FP.FromFloat(moveComp.Speed * SpeedAdaptive);
            var framePlay = new FramePlay_Move()
            {
                EntityId = entity.Id,
                Position = beforePos,
                AfterPosition = afterPos
            };
            return framePlay;
        }

        public static FramePlay_StopMove StopMoveFrame(EcsEntity entity)
        {
            var framePlay = new FramePlay_StopMove()
            {
                EntityId = entity.Id,
                LeftStopStep = entity.GetComponent<MoveComponent>().StopSpeed,
            };
            return framePlay;
        }

        public static FramePlay_MoveStop MoveStopFrame(EcsEntity entity, TSVector target, int leftStopStep)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            var beforePos = transComp.Position;
            var afterPos = transComp.Position + target * FP.FromFloat(moveComp.Speed * SpeedAdaptive);
            var framePlay = new FramePlay_MoveStop()
            {
                EntityId = entity.Id,
                LeftStopStep = leftStopStep,
                Position = beforePos,
                AfterPosition = afterPos
            };
            return framePlay;
        }

        public static FramePlay_StopMove StopMoveForecastFrame(EcsEntity entity)
        {
            var framePlay = new FramePlay_StopMove()
            {
                EntityId = entity.Id,
                LeftStopStep = entity.GetComponent<MoveComponent>().StopSpeed,
            };
            return framePlay;
        }

        public static FramePlay_MoveStop MoveForecastStopFrame(EcsEntity entity, TSVector target, int leftStopStep)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            var beforePos = transComp.ForecastPosition;
            var afterPos = transComp.ForecastPosition + target * FP.FromFloat(moveComp.Speed * SpeedAdaptive);
            var framePlay = new FramePlay_MoveStop()
            {
                EntityId = entity.Id,
                LeftStopStep = leftStopStep,
                Position = beforePos,
                AfterPosition = afterPos
            };
            return framePlay;
        }

        public static FramePlay_Move MoveForecastFrame(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            moveComp.ForecastTrueDirection = target;
            var beforePos = transComp.ForecastPosition;
            var afterPos = transComp.ForecastPosition + target * FP.FromFloat(moveComp.Speed * SpeedAdaptive);
            var framePlay = new FramePlay_Move()
            {
                EntityId = entity.Id,
                Position = beforePos,
                AfterPosition = afterPos
            };
            return framePlay;
        }

        public static void SetMovePosition(EcsEntity actor, TSVector position)
        {
            var transComp = actor.GetComponent<TransformComponent>();
            var beforePos = transComp.Position;
            TransformSystem.ChangePosition(actor, position);
        }

        public static void SetMoveForecastPosition(EcsEntity actor, TSVector position)
        {
            var transComp = actor.GetComponent<TransformComponent>();
            var beforePos = transComp.ForecastPosition;
            TransformSystem.ChangeForecastPosition(actor, position);
        }
    }
}
