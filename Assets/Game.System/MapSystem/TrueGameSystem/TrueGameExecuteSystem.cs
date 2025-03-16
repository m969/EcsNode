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
            var frame = game._currentFrame;
            if (!execute._inputQueue.ContainsKey(frame))
            {
                execute._inputQueue[frame] = new List<PlayerInput>();
            }
            input.Frame = frame;
            execute._inputQueue[frame].Add(input);
        }

        public static void FrameUpdate(TrueGame game, TrueGameExecuteComponent component)
        {
            var frame = game._currentFrame;

            // 取出当前帧所有输入
            component._inputQueue.TryGetValue(frame, out var inputs);
            component._inputQueue.Remove(frame);

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
                        case PlayerInputType.None:
                            break;
                        case PlayerInputType.Move:
                            MoveSystem.ChangeMove(actor, input.InputVector);
                            break;
                        case PlayerInputType.StopMove:
                            MoveSystem.ChangeMove(actor, TSVector.zero);
                            break;
                        case PlayerInputType.Look:
                            TrueTransformSystem.ChangeRotation(actor, input.InputVector);
                            break;
                        case PlayerInputType.Fire:
                            //FireSystem.ChangeFire(actor, input.InputVector);
                            FireSystem.Shoot(game, actor, input.InputVector);
                            break;
                        case PlayerInputType.StopFire:
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
