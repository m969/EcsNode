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
            var execute = game.GetComponent<TrueGameExecuteComponent>();
            var frame = game.CurrentFrame;
            if (!execute.InputQueue.ContainsKey(frame))
            {
                execute.InputQueue[frame] = new List<PlayerInput>();
            }
            input.Frame = frame;
            execute.InputQueue[frame].Add(input);
        }

        public static void FrameUpdate(TrueGame game, TrueGameExecuteComponent component)
        {
            var frame = game.CurrentFrame;

            // 取出当前帧所有输入
            component.InputQueue.TryGetValue(frame, out var inputs);
            component.InputQueue.Remove(frame);

            // 执行当前帧玩家所有输入
            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    var playerId = input.PlayerId;
                    var actor = game.GetChild<Actor>(playerId);

                    // 根据输入改变游戏状态
                    switch (input.InputType)
                    {
                        case InputType.None:
                            break;
                        case InputType.Move:
                            MoveSystem.ChangeMove(actor, input.InputVector);
                            break;
                        case InputType.StopMove:
                            MoveSystem.ChangeMove(actor, TSVector.zero);
                            break;
                        case InputType.Look:
                            TransformSystem.ChangeForward(actor, input.InputVector);
                            break;
                        case InputType.Fire:
                            //FireSystem.ChangeFire(actor, input.InputVector);
                            FireSystem.Shoot(game, actor, input.InputVector);
                            break;
                        case InputType.StopFire:
                            //FireSystem.StopFire(actor);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
