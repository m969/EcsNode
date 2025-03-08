using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TrueGameViewSystem : AEcsEntitySystem<TrueGame>,
    IAwake<TrueGame>,
    IInit<TrueGame>,
    IUpdate<TrueGame>
{
    public void Awake(TrueGame game)
    {
    }

    public void Init(TrueGame game)
    {
    }

    public void Update(TrueGame game)
    {
        // 收集本地玩家的输入操作
        if (game.GetComponent<PlayerInputComponent>() is { } inputComp)
        {
            PlayerInputSystem.Update(game, inputComp);

            // 帧播放前进后把收集到的输入填入游戏中
            if (game._currentInputFrame != game._currentFrame)
            {
                game._currentInputFrame = game._currentFrame;

                foreach (var item in inputComp.PlayerInputs)
                {
                    TrueGameExecuteSystem.AddPlayerInput(game, item);
                }

                inputComp.PlayerInputs.Clear();
            }
        }
    }
}
