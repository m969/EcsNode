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
            var advanceFrame = game.CurrentFrame + TrueGame.ForecastFrame;

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
                //TrueGameExecuteSystem.AddPlayerInput(game, input);

                //var execute = game.GetComponent<TrueGameExecuteComponent>();
                //var frame = game.CurrentFrame;
                //input.Frame = frame;
                //var actor = game.GetChild<Actor>(input.PlayerId);
                ActorPlaySystem.ProcessLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
