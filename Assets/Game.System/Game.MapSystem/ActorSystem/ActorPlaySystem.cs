using ECS;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorPlaySystem : AComponentSystem<Actor, FramePlayComponent>,
IAwake<Actor, FramePlayComponent>
    {
        public void Awake(Actor actor, FramePlayComponent component)
        {
            
        }

        public static void FrameUpdate(Actor actor, FramePlayComponent component, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            component.DetermineFrame = determineFrame;

            DetermineCreate(actor, determineFrame);
            if (game.MyActor == actor)
            {
                // 本地先行
                LocalAdvanceCreate(actor, determineFrame);
            }
            else
            {
                // 他人预测
                PredictCreate(actor, determineFrame);
            }
        }

        /// <summary>
        /// 播放运行帧序列改变游戏状态
        /// </summary>
        public static void PlayFramePlays(Actor actor, long determineFrame)
        {
            var component = actor.GetComponent<FramePlayComponent>();
            var moveComp = actor.GetComponent<MoveComponent>();
            foreach (var framePlay in component.FramePlays[determineFrame])
            {
                if (framePlay is FramePlay_Move movePlay)
                {
                    //moveComp.Moving = true;
                    MoveSystem.SetMovePosition(actor, movePlay.AfterPosition);
                    if (moveComp.LeftStopStep != 0)
                    {
                        moveComp.LeftStopStep = 0;
                    }
                }
                if (framePlay is FramePlay_StopMove stopMovePlay)
                {
                    //moveComp.Moving = false;
                    if (moveComp.LeftStopStep == 0)
                    {
                        moveComp.LeftStopStep = moveComp.StopSpeed;
                    }
                }
                if (framePlay is FramePlay_MoveStop moveStopPlay)
                {
                    moveComp.LeftStopStep--;
                    MoveSystem.SetMovePosition(actor, moveStopPlay.AfterPosition);
                }
            }
        }

        /// <summary>
        /// 根据输入创建行为帧，通过播放行为帧改变状态
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="determineFrame"></param>
        public static void DetermineCreate(Actor actor, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();

            // 取出确定帧输入
            component.DetermineFrameInputs.TryGetValue(determineFrame, out var inputs);
            var moveComp = actor.GetComponent<MoveComponent>();

            IFramePlay framePlay = null;

            if (!component.FramePlays.ContainsKey(determineFrame))
            {
                component.FramePlays[determineFrame] = new List<IFramePlay>();
            }
            var playList = component.FramePlays[determineFrame];

            // 执行确定帧输入
            if (inputs != null)
            {
                //ConsoleLog.Debug($"DetermineCreate {determineFrame}");
                foreach (var input in inputs)
                {
                    var inputType = input.InputType;
                    // 根据输入创建行为帧
                    if (inputType == InputType.Move)
                    {
                        framePlay = MoveSystem.MoveFrame(actor, input.InputVector);
                        playList.Add(framePlay);
                        //var movePlay = (FramePlay_Move)framePlay;
                        //ConsoleLog.Debug($"DetermineCreate {determineFrame} Move {input.InputVector} {movePlay.AfterPosition}");
                    }
                    if (inputType == InputType.StopMove)
                    {
                        framePlay = MoveSystem.StopMoveFrame(actor);
                        playList.Add(framePlay);
                        framePlay = MoveSystem.MoveStopFrame(actor, moveComp.TrueDirection);
                        playList.Add(framePlay);
                    }
                    //if (inputType == InputType.StopMove) MoveSystem.ChangeMove(actor, TSVector.zero);
                    if (inputType == InputType.Look) TransformSystem.ChangeForward(actor, input.InputVector);
                    if (inputType == InputType.Fire) FireSystem.FireOnce(actor, input.InputVector);
                }
            }

            if (moveComp.LeftStopStep > 0)
            {
                framePlay = MoveSystem.MoveStopFrame(actor, moveComp.TrueDirection);
                playList.Add(framePlay);
            }

            PlayFramePlays(actor, determineFrame);
            DetermineConflictCheck(actor, determineFrame);
        }

        public static void DetermineConflictCheck(Actor actor, long determineFrame)
        {
            var component = actor.GetComponent<FramePlayComponent>();
            if (component.AlreadyPredictFrame < determineFrame)
            {
                component.AlreadyPredictFrame = determineFrame;
                component.PredictionFramePlays.Clear();
            }

            component.FramePlays.TryGetValue(determineFrame, out var determineFramePlays);
            component.PredictionFramePlays.TryGetValue(determineFrame, out var predictionFramePlays);
            var needReset = false;
            var conflictType = string.Empty;
            if (predictionFramePlays == null)
            {
                needReset = true;
                component.PredictionFramePlays[determineFrame] = new List<IFramePlay>();
                conflictType = $"predictionFramePlays=null";
            }
            else
            {
                if (determineFramePlays.Count != predictionFramePlays.Count)
                {
                    needReset = true;
                    conflictType = $"determine={determineFramePlays.Count} prediction={predictionFramePlays.Count}";
                }
                else
                {
                    for (int i = 0; i < predictionFramePlays.Count; i++)
                    {
                        var prePlay = predictionFramePlays[i];
                        var determinePlay = determineFramePlays[i];
                        if (prePlay.GetType() != determinePlay.GetType())
                        {
                            needReset = true;
                            conflictType = $"{prePlay.GetType().Name} {determinePlay.GetType().Name}";
                            break;
                        }
                        if (prePlay.Equals(determinePlay) == false)
                        {
                            needReset = true;
                            conflictType = $"{prePlay.GetType().Name} {determinePlay.GetType().Name} !Equals";
                            //if (prePlay is FramePlay_MoveStop movePlay)
                            //{
                            //    ConsoleLog.Debug($"DetermineConflictCheck prePlay {movePlay.Position} {movePlay.AfterPosition}");
                            //}
                            //if (determinePlay is FramePlay_MoveStop movePlay2)
                            //{
                            //    ConsoleLog.Debug($"DetermineConflictCheck determinePlay {movePlay2.Position} {movePlay2.AfterPosition}");
                            //}
                            break;
                        }
                    }
                }
            }

            if (needReset)
            {
                component.ConflictType = conflictType;
                component.ConflictFrame = determineFrame;
                component.ConflictFrameCount++;
                component.AlreadyPredictFrame = determineFrame;
                component.PredictionFramePlays[determineFrame].Clear();
                component.PredictionFramePlays[determineFrame].AddRange(determineFramePlays);
                PredictionFramePlays(actor, determineFrame - 1);
                PredictionFramePlays(actor, determineFrame);
            }
        }

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
                        if (moveComp.ForecastLeftStopStep == 0)
                        {
                            moveComp.ForecastLeftStopStep = moveComp.StopSpeed;
                        }
                    }
                    if (framePlay is FramePlay_MoveStop moveStopPlay)
                    {
                        moveComp.ForecastLeftStopStep--;
                        MoveSystem.SetMoveForecastPosition(actor, moveStopPlay.AfterPosition);
                    }
                }
            }
        }

        public static void PredictCreate(Actor actor, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();
            var moveComp = actor.GetComponent<MoveComponent>();
            var predictFrame = determineFrame + TrueGame.ForecastFrame;
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
                            framePlay = MoveSystem.StopMoveFrame(actor);
                            playList.Add(framePlay);
                            framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection);
                            playList.Add(framePlay);
                        }
                    }
                }

                if (moveComp.ForecastLeftStopStep > 0)
                {
                    framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection);
                    playList.Add(framePlay);
                    //ConsoleLog.Debug($"PredictCreate {nowPredict} {moveComp.ForecastLeftStopStep}");
                }

                PredictionFramePlays(actor, nowPredict);
            }
        }

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
                            framePlay = MoveSystem.StopMoveFrame(actor);
                            playList.Add(framePlay);
                            framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection);
                            playList.Add(framePlay);
                        }
                    }
                }

                if (moveComp.ForecastLeftStopStep > 0)
                {
                    framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.ForecastTrueDirection);
                    playList.Add(framePlay);
                }

                PredictionFramePlays(actor, nowPredict);
            }
        }

        // 网络玩家输入
        public static void ProcessNetworkPlayerInput(Actor actor, PlayerInput input, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();

            if (component.DetermineFrameInputs.ContainsKey(determineFrame) == false)
            {
                component.DetermineFrameInputs.Add(determineFrame, new List<PlayerInput>());
            }
            component.DetermineFrameInputs[determineFrame].Add(input);
        }

        // 本地玩家输入
        public static void ProcessLocalPlayerInput(Actor actor, PlayerInput input, long advanceFrame)
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
