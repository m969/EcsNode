using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class ChaseAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            ConsoleLog.Debug("ChaseAIAction Awake");
            //MoveSystem.ChangeDirection(aiNode.Entity, TSVector.zero);
            //MoveSystem.StartMove(aiNode.Entity);
        }

        public void Update(AINode aiNode)
        {
            MoveSystem.ChangeDirection(aiNode.Entity, TSVector.zero);
        }
    }
}
