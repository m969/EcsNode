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

        public static void SetSpeed(EcsEntity entity, int speed)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Speed = speed;
        }

        public static void ChangeMove(EcsEntity entity, TSVector target)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.TrueDirection = target;
        }

        public static void SetMovePosition(EcsEntity actor, TSVector position)
        {
            var transComp = actor.GetComponent<TrueTransformComponent>();
            var beforePos = transComp.Position;
            transComp.Position = position;

            EventSystem.Dispatch(actor.EcsNode, new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<MoveComponent>(),
            });
        }
    } 
}
