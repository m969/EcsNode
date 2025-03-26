using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class MoveAIAction: IAIAction
    {
        public void Start(AINode aiNode)
        {
            //ConsoleLog.Debug("MoveAIAction Start");
            var game = aiNode.Entity.GetParent<TrueGame>();
            var r = game.TSRandom.Next(1, 5);
            var vector = TSVector.left;
            if (r == 1) vector = TSVector.left;
            if (r == 2) vector = TSVector.right;
            if (r == 3) vector = TSVector.forward;
            if (r == 4) vector = TSVector.back;
            TrueGameExecuteSystem.AddPlayerInput(game, new PlayerInput() { InputType = InputType.Move, InputVector = vector, PlayerId = aiNode.Entity.Id });
            AISystem.FinishAndNext(aiNode);
        }

        public void Run(AINode aiNode)
        {

        }
    }
}
