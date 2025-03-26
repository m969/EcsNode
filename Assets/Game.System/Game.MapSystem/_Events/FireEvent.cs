using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class FireEvent : AEventRun<FireEvent, Actor, TSVector>
    {
        protected override async ETTask Run(Actor actor, TSVector direction)
        {
            var game = actor.GetParent<TrueGame>();
            var item = game.AddChild<Item>();
            item.AddComponent<TransformComponent>();
            item.AddComponent<MoveComponent>();
            var actorTrans = actor.GetComponent<TransformComponent>();
            TransformSystem.ChangePosition(item, actorTrans.Position);
            TransformSystem.ChangeForward(item, actorTrans.Forward);
            MoveSystem.SetSpeed(item, 15);
            MoveSystem.ChangeMove(item, actorTrans.Forward.normalized);

            EventSystem.Dispatch(new EntityCreateCmd()
            {
                Entity = item
            });
        }
    }
}
