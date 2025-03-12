using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

public class TrueGameExecuteSystem : AEcsComponentSystem<TrueGame, TrueGameExecuteComponent>,
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
        //ConsoleLog.Log("TrueGameExecuteSystem AddPlayerInput");
        var execute = game.GetComponent<TrueGameExecuteComponent>();
        var frame = game._currentFrame;
        //ConsoleLog.Log("TrueGameExecuteSystem AddPlayerInput2");
        //lock (execute._locker)
        {
            if (!execute._inputQueue.ContainsKey(frame))
            {
                execute._inputQueue[frame] = new List<PlayerInput>();
            }
            input.Frame = frame;
            execute._inputQueue[frame].Add(input);
            //ConsoleLog.Log("TrueGameExecuteSystem AddPlayerInput _locker");
        }
    }

    public static void Update(TrueGame game, TrueGameExecuteComponent component)
    {
        var frame = game._currentFrame;

        // 取出当前帧所有输入
        List<PlayerInput> inputs;
        //lock (component._locker)
        {
            component._inputQueue.TryGetValue(frame, out inputs);
            component._inputQueue.Remove(frame);
        }

        // 执行当前帧玩家所有输入
        if (inputs != null)
        {
            foreach (var input in inputs)
            {
                var playerId = input.PlayerId;
                var actor = game.GetChild<Actor>(playerId);
                MoveSystem.ChangeMove(actor, TSVector.right);
            }
        }
    }
}
