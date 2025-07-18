using ECS;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorAdvancePlaySystem : AComponentSystem<Actor, FramePlayComponent>
    {
        public static void LocalAdvanceCreate(Actor actor, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();
            var moveComp = actor.GetComponent<MoveComponent>();
            var advanceFrame = determineFrame + TrueGame.ForecastFrame;
            var alreadyPredictFrame = component.AlreadyPredictFrame;
            var nextPredict = alreadyPredictFrame + 1;
            //ConsoleLog.Debug($"LocalAdvanceCreate {advanceFrame}");

            for (var i = nextPredict; i <= advanceFrame; i++)
            {
                var nowPredict = i;
                component.AlreadyPredictFrame = nowPredict;
                component.PredictionFramePlays[nowPredict] = new List<IFramePlay>();

                // 取出先行帧输入
                component.AdvanceFrameInputs.TryGetValue(nowPredict, out var inputs);

                if (!component.PredictionFramePlays.ContainsKey(nowPredict))
                {
                    component.PredictionFramePlays[nowPredict] = new List<IFramePlay>();
                }
                var playList = component.PredictionFramePlays[nowPredict];

                IFramePlay framePlay = null;

                // 执行先行帧输入
                if (inputs != null)
                {
                    foreach (var input in inputs)
                    {
                        var inputType = input.InputType;
                        // 根据输入立即创建预测运行帧
                        if (inputType == InputType.Move)
                        {
                            framePlay = MoveSystem.MoveForecastFrame(actor, input.InputVector);
                            playList.Add(framePlay);
                        }
                        if (inputType == InputType.StopMove)
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
                }

                ActorPredictPlaySystem.PredictionFramePlays(actor, nowPredict);
            }
        }

        // 本地玩家输入
        public static void AddLocalPlayerInput(Actor actor, PlayerInput input, long advanceFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();

            if (component.AdvanceFrameInputs.ContainsKey(advanceFrame) == false)
            {
                component.AdvanceFrameInputs.Add(advanceFrame, new List<PlayerInput>());
            }
            component.AdvanceFrameInputs[advanceFrame].Add(input);
        }
    }
}
