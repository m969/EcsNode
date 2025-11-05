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
        public void Awake(AINode aiNode)
        {
            var game = aiNode.Entity.GetParent<TrueWorld>();
            var actor = (Actor)aiNode.Entity;
            var component = actor.GetComponent<AIComponent>();
            var input = new InputData() { InputType = InputType.StopMove, PlayerId = aiNode.Entity.Id };
            ActorPredictPlaySystem.AddNetworkPlayerInput(actor, input, component.DetermineFrame);
            aiNode.StartAction<WaitAIAction>();
        }

        public void Update(AINode aiNode)
        {

        }
    }
}
