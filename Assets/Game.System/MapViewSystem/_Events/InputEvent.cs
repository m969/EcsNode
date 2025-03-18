using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class InputEvent : AEventRun<TrueGame, InputType, Vector3>
    {
        protected override async ETTask Run(TrueGame game, InputType inputType, Vector3 direction)
        {
            var component = game.GetComponent<PlayerInputComponent>();

            if (inputType == InputType.Fire)
            {
                component.FireVector = direction;
                var input = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = InputType.Fire,
                    InputVector = direction.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, input);
            }
        }
    }
}
