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
            //ConsoleLog.Debug("ActorPlaySystem");
            var game = actor.GetParent<TrueGame>();
            //var frame = game.DetermineFrame;

            DetermineCreate(actor, determineFrame);
            if (game.MyActor == actor)
            {
                // 本地先行
                LocalAdvanceCreate(actor, determineFrame);
                //var transComp = actor.GetComponent<TransformComponent>();
                //ConsoleLog.Debug($"{transComp.Position} {transComp.ForecastPosition}");
            }
            else
            {
                // 他人预测
                PredictCreate(actor, determineFrame);
            }
        }

        //public static IFramePlay AddFramePlay(Actor actor, InputType playType, long determineFrame)
        //{
        //    var game = actor.GetParent<TrueGame>();
        //    var component = actor.GetComponent<FramePlayComponent>();
        //    IFramePlay framePlay = null;

        //    if (playType == InputType.Move)
        //    {
        //        var moveComp = actor.GetComponent<MoveComponent>();
        //        var transComp = actor.GetComponent<TransformComponent>();
        //        var beforePos = transComp.Position;
        //        var afterPos = transComp.Position + moveComp.TrueDirection * FP.FromFloat(moveComp.Speed * 0.1f);
        //        framePlay = new FramePlay_Move()
        //        {
        //            EntityId = actor.Id,
        //            Position = beforePos,
        //            AfterPosition = afterPos
        //        };
        //    }

        //    if (framePlay != null)
        //    {
        //        component.FramePlays[determineFrame].Add(framePlay);
        //    }

        //    return framePlay;
        //}

        /// <summary>
        /// 根据游戏状态创建运行帧
        /// </summary>
        //public static void CreateFramePlays(Actor actor, long determineFrame)
        //{
        //    var component = actor.GetComponent<FramePlayComponent>();
        //    if (actor.GetComponent<MoveComponent>() is { } moveComponent)
        //    {
        //        if (moveComponent.TrueDirection != TSVector.zero)
        //        {
        //            AddFramePlay(actor, StatePlayType.Move, determineFrame);
        //        }
        //    }
        //}

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
            //component.DetermineFrameInputs.Remove(determineFrame);
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
            DetermineConfictCheck(actor, determineFrame);
        }

        public static void DetermineConfictCheck(Actor actor, long determineFrame)
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
            if (predictionFramePlays == null)
            {
                needReset = true;
                component.PredictionFramePlays[determineFrame] = new List<IFramePlay>();
                //ConsoleLog.Debug($"DetermineConfictCheck predictionFramePlays null");
            }
            else
            {
                if (determineFramePlays.Count != predictionFramePlays.Count)
                {
                    needReset = true;
                    //ConsoleLog.Debug($"DetermineConfictCheck Count false");
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
                            break;
                        }
                        if (prePlay.Equals(determinePlay) == false)
                        {
                            needReset = true;
                            //ConsoleLog.Debug($"DetermineConfictCheck Equals false");
                            break;
                        }
                    }
                }
            }

            if (needReset)
            {
                component.AlreadyPredictFrame = determineFrame;
                component.PredictionFramePlays[determineFrame].Clear();
                component.PredictionFramePlays[determineFrame].AddRange(determineFramePlays);
                PredictionFramePlays(actor, determineFrame - 1);
                PredictionFramePlays(actor, determineFrame);
                //ConsoleLog.Debug($"DetermineConfictCheck needReset {determineFrame} {TransformSystem.GetPosition(actor)} {TransformSystem.GetForecastPosition(actor)}");
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

                if (!component.PredictionFramePlays.ContainsKey(nowPredict))
                {
                    component.PredictionFramePlays[nowPredict] = new List<IFramePlay>();
                }
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
                            //var movePlay = (FramePlay_Move)framePlay;
                            //ConsoleLog.Debug($"{determineFrame} {nowPredict} Move {input.InputVector} {movePlay.AfterPosition}");
                        }
                        if (inputType == InputType.StopMove)
                        {
                            framePlay = MoveSystem.StopMoveFrame(actor);
                            playList.Add(framePlay);
                            framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.TrueDirection);
                            playList.Add(framePlay);
                        }
                    }
                }

                if (moveComp.ForecastLeftStopStep > 0)
                {
                    framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.TrueDirection);
                    playList.Add(framePlay);
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

            //List<PlayerInput> lastInputs = null;
            for (var i = nextPredict; i <= advanceFrame; i++)
            {
                var nowPredict = i;
                component.AlreadyPredictFrame = nowPredict;
                //ConsoleLog.Debug($"LocalAdvanceCreate {determineFrame} {advanceFrame} {nowPredict}");
                if (!component.PredictionFramePlays.ContainsKey(nowPredict))
                {
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
                                //var movePlay = (FramePlay_Move)framePlay;
                                //ConsoleLog.Debug($"LocalAdvanceCreate {nowPredict} Move {input.InputVector} {movePlay.AfterPosition}");
                            }
                            if (inputType == InputType.StopMove)
                            {
                                framePlay = MoveSystem.StopMoveFrame(actor);
                                playList.Add(framePlay);
                                framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.TrueDirection);
                                playList.Add(framePlay);
                            }
                        }
                    }

                    if (moveComp.ForecastLeftStopStep > 0)
                    {
                        framePlay = MoveSystem.MoveForecastStopFrame(actor, moveComp.TrueDirection);
                        playList.Add(framePlay);
                    }

                    PredictionFramePlays(actor, nowPredict);
                }
            }
        }

        // 网络玩家输入
        public static void ProcessNetworkPlayerInput(Actor actor, PlayerInput input, long determineFrame)
        {
            var game = actor.GetParent<TrueGame>();
            var component = actor.GetComponent<FramePlayComponent>();
            //var determineFrame = game.DetermineFrame;

            if (component.DetermineFrameInputs.ContainsKey(determineFrame) == false)
            {
                component.DetermineFrameInputs.Add(determineFrame, new List<PlayerInput>());
            }
            component.DetermineFrameInputs[determineFrame].Add(input);
            //ConsoleLog.Debug($"ProcessNetworkPlayerInput {determineFrame}");

            //if (component.AdvanceFrameInputs.ContainsKey(determineFrame) == false)
            //{
            //    component.AdvanceFrameInputs.Add(determineFrame, new List<PlayerInput>());
            //}
            //component.AdvanceFrameInputs[determineFrame].Add(input);
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
            //ConsoleLog.Debug($"ProcessLocalPlayerInput {advanceFrame} {component.AdvanceFrameInputs[advanceFrame].Count}");
        }
    }
}
