using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class InputEvent : AEventRun<InputEvent, TrueGame, InputType, Vector3>
    {
        protected override async ETTask Run(TrueGame game, InputType inputType, Vector3 direction)
        {
            var component = game.GetComponent<PlayerInputComponent>();
            var myActor = game.MyActor;
            var advanceFrame = game.DetermineFrame + TrueGame.ForecastFrame;

            if (inputType == InputType.Fire)
            {
                component.FireVector = direction;
                var input = new PlayerInput()
                {
                    Frame = advanceFrame,
                    PlayerId = myActor.Id,
                    InputType = InputType.Fire,
                    InputVector = direction.ToTSVector(),
                };

                ActorPlaySystem.ProcessLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
