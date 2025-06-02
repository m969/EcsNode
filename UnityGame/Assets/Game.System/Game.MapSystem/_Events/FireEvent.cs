using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class FireEvent : AEventRun<Actor, TSVector>
    {
        public override EcsNode EcsNode { get; set; }
        public Actor Actor { get; private set; }
        public TSVector Direction { get; private set; }

        protected override async ETTask Run(Actor actor, TSVector direction)
        {
            Actor = actor;
            Direction = direction;

            var game = actor.GetParent<TrueGame>();
            var item = game.AddChild<Item>();
            item.AddComponent<TransformComponent>();
            item.AddComponent<CollisionComponent>();
            item.AddComponent<MoveComponent>();
            item.GetComponent<CollisionComponent>().Layer = actor.GetComponent<CollisionComponent>().Layer;
            var actorTrans = actor.GetComponent<TransformComponent>();
            TransformSystem.ChangePosition(item, actorTrans.ForecastPosition);
            TransformSystem.ChangeForward(item, actorTrans.Forward);
            MoveSystem.SetSpeed(item, 150);
            MoveSystem.ChangeMove(item, actorTrans.Forward.normalized);
            item.Init();
        }
    }
}
