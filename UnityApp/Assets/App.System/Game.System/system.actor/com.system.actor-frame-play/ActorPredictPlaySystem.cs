using ECS;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorPredictPlaySystem : AComponentSystem<Actor, FramePlayComponent>
    {
        /// <summary>
        /// 播放预测运行帧序列
        /// </summary>
        public static void PredictionFramePlays(Actor actor, long frame)
        {
            var component = actor.GetComponent<FramePlayComponent>();
            var moveComp = actor.GetComponent<MoveComponent>();
            component.PredictionFramePlays.TryGetValue(frame, out var framePlays);
            if (framePlays != null)
            {
                foreach (var framePlay in framePlays)
                {
                    if (framePlay is FramePlay_Move movePlay)
                    {
                        MoveSystem.SetMoveForecastPosition(actor, movePlay.AfterPosition);
                        if (moveComp.ForecastLeftStopStep != 0)
                        {
                            moveComp.ForecastLeftStopStep = 0;
                        }
                    }
                    if (framePlay is FramePlay_StopMove stopMovePlay)
                    {
                        moveComp.ForecastLeftStopStep = stopMovePlay.LeftStopStep;
                        //if (moveComp.ForecastLeftStopStep == 0)
                        //{
                        //    moveComp.ForecastLeftStopStep = moveComp.StopSpeed;
                        //}
                    }
                    if (framePlay is FramePlay_MoveStop moveStopPlay)
                    {
                        moveComp.ForecastLeftStopStep = moveStopPlay.LeftStopStep;
                        //moveComp.ForecastLeftStopStep--;
                        MoveSystem.SetMoveForecastPosition(actor, moveStopPlay.AfterPosition);
                        //ConsoleLog.Debug($"ActorPredictPlaySystem PredictionFramePlays {frame} {moveStopPlay.AfterPosition}");
                    }
                }
            }
        }

        public static void PredictCreate(Actor actor, long determineFrame)
        {
            var game = actor.GetParent<TrueWorld>();
            var component = actor.GetComponent<FramePlayComponent>();
            var moveComp = actor.GetComponent<MoveComponent>();
            var predictFrame = determineFrame + TrueWorld.ForecastFrame;
            var alreadyPredictFrame = component.AlreadyPredictFrame;
            var nextPredict = alreadyPredictFrame + 1;

            component.DetermineFrameInputs.TryGetValue(determineFrame, out var lastInputs);
            for (var i = nextPredict; i <= predictFrame; i++)
            {
                var nowPredict = i;
                component.AlreadyPredictFrame = nowPredict;

                component.PredictionFramePlays[nowPredict] = new List<IFramePlay>();
                var playList = component.PredictionFramePlays[nowPredict];

                IFramePlay framePlay = null;

                // 执行先行帧输入
                if (lastInputs != null)
                {
                    foreach (var input in lastInputs)
                    {
                        var inputType = input.InputType;
                        // 根据输入立即创建预测运行帧
                        if (inputType == InputType.Move)
                        {
                            framePlay = MoveSystem.MoveForecastFrame(actor, input.InputVector);
                            playList.Add(framePlay);
                        }
                        if (inputType == InputType.StopMove && moveComp.ForecastLeftStopStep == 0)
                        {
                            //framePlay = MoveSystem.StopMoveForecastFrame(actor);
                            //playList.Add(framePlay);
                            framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection, moveComp.StopSpeed - 1);
                            playList.Add(framePlay);
                        }
                    }
                }

                if (moveComp.ForecastLeftStopStep > 0)
                {
                    framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection, moveComp.ForecastLeftStopStep - 1);
                    playList.Add(framePlay);
                    //ConsoleLog.Debug($"PredictCreate {nowPredict} {moveComp.ForecastLeftStopStep}");
                }

                PredictionFramePlays(actor, nowPredict);
            }
        }

        // 网络玩家输入
        public static void AddNetworkPlayerInput(Actor actor, InputData input, long determineFrame)
        {
            var game = actor.GetParent<TrueWorld>();
            var component = actor.GetComponent<FramePlayComponent>();

            if (component.DetermineFrameInputs.ContainsKey(determineFrame) == false)
            {
                component.DetermineFrameInputs.Add(determineFrame, new List<InputData>());
            }
            component.DetermineFrameInputs[determineFrame].Add(input);
        }
    }
}
