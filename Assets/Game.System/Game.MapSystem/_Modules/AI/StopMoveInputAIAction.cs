using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class StopMoveInputAIAction : IAIAction
    {
        public void Start(AINode aiNode)
        {
            //ConsoleLog.Debug("IdleAIAction Start");
            var game = aiNode.Entity.GetParent<TrueGame>();
            //TrueGameExecuteSystem.AddPlayerInput(game, );
            var actor = (Actor)aiNode.Entity;
            var component = actor.GetComponent<AIComponent>();
            var input = new PlayerInput() { InputType = InputType.StopMove, PlayerId = aiNode.Entity.Id };
            ActorPlaySystem.ProcessNetworkPlayerInput(actor, input, component.DetermineFrame);
            AISystem.FinishAndNext(aiNode);
        }

        public void Run(AINode aiNode)
        {

        }
    }
}
