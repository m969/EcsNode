using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class IdleAIAction : IAIAction
    {
        public void Start(AINode aiNode)
        {
            //ConsoleLog.Debug("IdleAIAction Start");
            var game = aiNode.Entity.GetParent<TrueGame>();
            TrueGameExecuteSystem.AddPlayerInput(game, new PlayerInput() { InputType = InputType.StopMove, PlayerId = aiNode.Entity.Id });
            AISystem.FinishAndNext(aiNode);
        }

        public void Run(AINode aiNode)
        {

        }
    }
}
