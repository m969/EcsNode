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

                // 帧播放向前推进一帧，把收集到的输入填入最新的一帧游戏中
                if (game.CurrentInputFrame != game.CurrentFrame)
                {
                    game.CurrentInputFrame = game.CurrentFrame;

                    FrameUpdate(game, inputComp);
                }
            }
        }

        public void FrameUpdate(TrueGame game, PlayerInputComponent inputComp)
        {
            if (inputComp.LookVector != Vector3.zero)
            {
                var input = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = InputType.Look,
                    InputVector = inputComp.LookVector.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, input);
            }

            if (inputComp.MoveVector != Vector3.zero)
            {
                var input = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = InputType.Move,
                    InputVector = inputComp.MoveVector.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, input);
            }

            //if (inputComp.FireState)
            //{
            //    var input = new PlayerInput()
            //    {
            //        PlayerId = game.MyActor.Id,
            //        InputType = PlayerInputType.Fire,
            //        InputVector = inputComp.FireVector.ToTSVector(),
            //    };
            //    TrueGameExecuteSystem.AddPlayerInput(game, input);
            //}

            foreach (var item in inputComp.PlayerInputs)
            {
                TrueGameExecuteSystem.AddPlayerInput(game, item);
            }

            inputComp.PlayerInputs.Clear();
        }
    }
}
