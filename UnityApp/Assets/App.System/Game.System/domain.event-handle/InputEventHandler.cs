using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class InputEventHandler : AEventRun<PlayerInput, InputEvent>
    {
        protected override async ETTask Run(PlayerInput component, InputEvent inputEvent)
        {
            var myActor = component.PlayerActor;
            var advanceFrame = component.Game.DetermineFrame + TrueGame.ForecastFrame;

            if (inputEvent.InputType == InputType.Fire)
            {
                component.FireVector = inputEvent.Direction;
                var input = new InputData()
                {
                    Frame = advanceFrame,
                    PlayerId = myActor.Id,
                    InputType = InputType.Fire,
                    InputVector = inputEvent.Direction.ToTSVector(),
                };

                ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
