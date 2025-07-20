using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class InputExecuteEvent : AEventRun<PlayerInput, InputType, Vector3>
    {
        protected override async ETTask Run(PlayerInput component, InputType inputType, Vector3 direction)
        {
            var myActor = component.PlayerActor;
            var advanceFrame = component.Game.DetermineFrame + TrueGame.ForecastFrame;

            if (inputType == InputType.Fire)
            {
                component.FireVector = direction;
                var input = new InputData()
                {
                    Frame = advanceFrame,
                    PlayerId = myActor.Id,
                    InputType = InputType.Fire,
                    InputVector = direction.ToTSVector(),
                };

                ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
