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
        public void Start(AINode aiNode)
        {
            //ConsoleLog.Debug("WaitAIAction Start");
        }

        public void Run(AINode aiNode)
        {
            //ConsoleLog.Debug("WaitAIAction Run");
            var timerProgress = TimerSystem.FrameTimer(aiNode.Entity, TimerType.FrameTimer, 20);
            if (timerProgress == TimerProgress.Ended)
            {
                //ConsoleLog.Debug("WaitAIAction Run Ended");
                AISystem.FinishProcess(aiNode);
            }
        }
    }
}
