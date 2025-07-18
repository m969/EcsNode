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
            game.OnFrameUpdate += FrameUpdate;
        }

        public void Update(TrueGame game)
        {
            // 收集本地玩家的输入操作
            if (game.GetComponent<PlayerInputComponent>() is { } inputComp)
            {
                PlayerInputSystem.Update(game, inputComp);
            }
        }

        public static void FrameUpdate(TrueGame game, long determineFrame)
        {
            var myActor = game.MyActor;
            var inputComp = game.GetComponent<PlayerInputComponent>();
            var newInputFrame = determineFrame + TrueGame.ForecastFrame;
            PlayerInputSystem.FrameUpdate(game, inputComp, determineFrame, newInputFrame);
        }
    }
}
