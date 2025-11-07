using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class WaitAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            //ConsoleLog.Debug("WaitAIAction Start");
        }

        public void Update(AINode aiNode)
        {
            var timerProgress = FrameTimerSystem.FrameTimer(aiNode.Entity, TimerType.WaitAIAction_FrameTimer, 20);
            if (timerProgress == TimerProgress.Ended)
            {
                AISystem.MoveNext(aiNode);
            }
        }
    }
}
