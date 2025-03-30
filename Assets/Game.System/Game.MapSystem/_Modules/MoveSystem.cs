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

        public static FramePlay_Move MoveFrame(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            moveComp.TrueDirection = target;
            var beforePos = transComp.Position;
            var afterPos = transComp.Position + target * FP.FromFloat(moveComp.Speed * 0.1f);
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
            };
            return framePlay;
        }

        public static FramePlay_MoveStop MoveStopFrame(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            var beforePos = transComp.Position;
            var afterPos = transComp.Position + target * FP.FromFloat(moveComp.Speed * 0.1f);
            var framePlay = new FramePlay_MoveStop()
            {
                EntityId = entity.Id,
                Position = beforePos,
                AfterPosition = afterPos
            };
            return framePlay;
        }

        public static FramePlay_Move MoveForecastFrame(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            var transComp = entity.GetComponent<TransformComponent>();
            var beforePos = transComp.ForecastPosition;
            var afterPos = transComp.ForecastPosition + target * FP.FromFloat(moveComp.Speed * 0.1f);
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
            transComp.Position = position;

            //EventSystem.Dispatch(new EntityUpdateCmd()
            //{
            //    Entity = actor,
            //    ChangeComponent = actor.GetComponent<MoveComponent>(),
            //});
        }

        public static void SetForecastPosition(EcsEntity actor, TSVector position)
        {
            var transComp = actor.GetComponent<TransformComponent>();
            var beforePos = transComp.ForecastPosition;
            transComp.ForecastPosition = position;

            EventSystem.Dispatch(new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<MoveComponent>(),
            });
        }
    } 
}
