using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using ECSGame.ChaseModule;

namespace ECSGame
{
    public class MoveToAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            var direction = TSVector.Normalize(MoveSystem.GetDestination(aiNode.Entity) - TransformSystem.GetPosition(aiNode.Entity));
            MoveSystem.ChangeDirection(aiNode.Entity, direction);
            TransformSystem.ChangeForward(aiNode.Entity, direction);
            MoveSystem.StartMove(aiNode.Entity);
        }

        public void Update(AINode aiNode)
        {
            var distance = Vector3.Distance(MoveSystem.GetDestination(aiNode.Entity).ToVector(), TransformSystem.GetPosition(aiNode.Entity).ToVector());
            if (distance <= 0.1f)
            {
                MoveSystem.StopMove(aiNode.Entity);
                AISystem.MoveNext(aiNode);
            }
        }
    }
}
