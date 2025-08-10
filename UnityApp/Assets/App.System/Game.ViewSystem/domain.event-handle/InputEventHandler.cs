using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using ECSGame;

namespace ECSUnity
{
    public class InputEventHandler : AEventRun<PlayerInput, InputEvent>
    {
        protected override async ETTask Run(PlayerInput playerInput, InputEvent inputEvent)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }

            var myActor = component.PlayerActor;
            var advanceFrame = component.TrueWorld.DetermineFrame + TrueWorld.ForecastFrame;

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
