using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class FrameTimerSystem : AComponentSystem<EcsNode, TimerComponent>,
IAwake<EcsNode, TimerComponent>
    {
        public void Awake(EcsNode entity, TimerComponent component)
        {
        }

        public static void Update(EcsNode entity, TimerComponent component)
        {

        }

        public static TimerProgress FrameTimer(EcsEntity entity, int timerType, long frameCount)
        {
            if (entity.IsDisposed)
            {
                return TimerProgress.Disposed;
            }

            var ecsNode = entity.EcsNode;
            var timerComp = ecsNode.GetComponent<TimerComponent>();
            var game = entity.GetParent<TrueWorld>();

            if (timerComp.FrameTimers.ContainsKey(timerType))
            {
                var result = timerComp.FrameTimers[timerType].EndFrame <= game.DetermineFrame;
                if (result)
                {
                    timerComp.FrameTimers.Remove(timerType);
                    return TimerProgress.Ended;
                }
            }
            else
            {
                timerComp.FrameTimers.Add(timerType, new FrameTimer()
                {
                    EndFrame = game.DetermineFrame + frameCount,
                    TimerFrame = frameCount,
                    Repeat = false
                });
            }

            return TimerProgress.Waiting;
        }

        public static TimerProgress FrameRepeatTimer(EcsEntity entity, int timerType, long frameCount, bool preTrigger = false)
        {
            if (entity.IsDisposed)
            {
                return TimerProgress.Disposed;
            }

            var ecsNode = entity.EcsNode;
            var timerComp = ecsNode.GetComponent<TimerComponent>();
            var game = entity.GetParent<TrueWorld>();

            if (timerComp.FrameTimers.ContainsKey(timerType))
            {
                var frameTimer = timerComp.FrameTimers[timerType];
                var result = frameTimer.EndFrame <= game.DetermineFrame;
                if (result)
                {
                    if (frameTimer.Repeat)
                    {
                        frameTimer.EndFrame = game.DetermineFrame + frameCount;
                    }
                    return TimerProgress.Ended;
                }
            }
            else
            {
                timerComp.FrameTimers.Add(timerType, new FrameTimer()
                {
                    EndFrame = game.DetermineFrame + frameCount,
                    TimerFrame = frameCount,
                    Repeat = true
                });
            }

            if (preTrigger)
            {
                return TimerProgress.Ended;
            }

            return TimerProgress.Waiting;
        }
    } 
}
