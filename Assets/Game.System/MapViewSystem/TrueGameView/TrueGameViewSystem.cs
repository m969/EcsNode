using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class TrueGameViewSystem : AEntitySystem<TrueGame>,
IAwake<TrueGame>,
IInit<TrueGame>,
IUpdate<TrueGame>
    {
        public void Awake(TrueGame game)
        {
        }

        public void Init(TrueGame game)
        {
        }

        public void Update(TrueGame game)
        {
            // 收集本地玩家的输入操作
            if (game.GetComponent<PlayerInputComponent>() is { } inputComp)
            {
                PlayerInputSystem.Update(game, inputComp);

                // 帧播放向前推进后，把收集到的输入填入游戏中
                if (game._currentInputFrame != game._currentFrame)
                {
                    game._currentInputFrame = game._currentFrame;

                    FrameUpdate(game, inputComp);
                }
            }
        }

        public void FrameUpdate(TrueGame game, PlayerInputComponent inputComp)
        {
            if (inputComp.LookVector != Vector3.zero)
            {
                var moveInput = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = PlayerInputType.Look,
                    InputVector = inputComp.LookVector.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, moveInput);
            }

            if (inputComp.MoveVector != Vector3.zero)
            {
                var moveInput = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = PlayerInputType.Move,
                    InputVector = inputComp.MoveVector.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, moveInput);
            }

            foreach (var item in inputComp.PlayerInputs)
            {
                TrueGameExecuteSystem.AddPlayerInput(game, item);
            }

            inputComp.PlayerInputs.Clear();
        }
    }
}
