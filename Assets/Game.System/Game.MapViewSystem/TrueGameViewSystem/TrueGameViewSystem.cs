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

                var myActor = game.MyActor;
                var determineFrame = game.DetermineFrame;
                var advanceFrame = determineFrame + TrueGame.ForecastFrame;

                // 帧播放向前推进一帧，把收集到的输入填入最新的一帧中
                if (myActor.GetComponent<FramePlayComponent>().CurrentInputFrame != advanceFrame)
                {
                    myActor.GetComponent<FramePlayComponent>().CurrentInputFrame = advanceFrame;
                    PlayerInputSystem.FrameUpdate(game, inputComp, determineFrame, advanceFrame);
                }
            }
        }

        //public static void FrameUpdate(TrueGame game, PlayerInputComponent inputComp)
        //{
        //}
    }
}
