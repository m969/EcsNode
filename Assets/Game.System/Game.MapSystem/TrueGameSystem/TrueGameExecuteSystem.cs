using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class TrueGameExecuteSystem : AComponentSystem<TrueGame, TrueGameExecuteComponent>,
IAwake<TrueGame, TrueGameExecuteComponent>,
IInit<TrueGame, TrueGameExecuteComponent>
    {
        public void Awake(TrueGame game, TrueGameExecuteComponent component)
        {
        }

        public void Init(TrueGame game, TrueGameExecuteComponent component)
        {
        }

        // 添加玩家输入
        public static void AddPlayerInput(TrueGame game, PlayerInput input)
        {
            //var execute = game.GetComponent<TrueGameExecuteComponent>();
            //var frame = game.DetermineFrame;
            //input.Frame = frame;
            //var actor = game.GetChild<Actor>(input.PlayerId);
            //ActorPlaySystem.AddPlayerInput(actor, input);

            //if (!execute.InputQueue.ContainsKey(frame))
            //{
            //    execute.InputQueue[frame] = new List<PlayerInput>();
            //}
            //execute.InputQueue[frame].Add(input);
        }

        public static void FrameUpdate(TrueGame game, TrueGameExecuteComponent component, long determineFrame)
        {
            //var frame = game.CurrentFrame;

            //// 取出当前帧所有输入
            //component.InputQueue.TryGetValue(frame, out var inputs);
            //component.InputQueue.Remove(frame);

            //// 执行当前帧玩家所有输入
            //if (inputs != null)
            //{
            //    foreach (var input in inputs)
            //    {
            //        var playerId = input.PlayerId;
            //        var actor = game.GetChild<Actor>(playerId);

            //        var inputType = input.InputType;
            //        // 根据输入改变游戏状态
            //        if (inputType == InputType.Move) MoveSystem.ChangeMove(actor, input.InputVector);
            //        if (inputType == InputType.StopMove) MoveSystem.ChangeMove(actor, TSVector.zero);
            //        if (inputType == InputType.Look) TransformSystem.ChangeForward(actor, input.InputVector);
            //        if (inputType == InputType.Fire) FireSystem.FireOnce(actor, input.InputVector);
            //    }
            //}
        }
    }
}
