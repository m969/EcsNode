using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class TrueGameSystem : AEntitySystem<TrueGame>,
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

        // 帧同步核心逻辑
        public void Update(TrueGame game)
        {
            // 计算下一帧理论执行时间
            long nextFrameTime = game._currentFrame * game._frameInterval;

            // 等待到下一帧时间
            while (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond < nextFrameTime)
            {
                return;
            }

            // 处理当前帧逻辑
            {
                // 执行玩家输入，影响游戏运行状态
                if (game.GetComponent<TrueGameExecuteComponent>() is { } component)
                {
                    TrueGameExecuteSystem.FrameUpdate(game, component);
                }

                // 模拟播放游戏运行
                if (game.GetComponent<TrueGamePlayComponent>() is { } component2)
                {
                    TrueGamePlaySystem.FrameUpdate(game, component2);
                }
            }

            // 推进帧数
            game._currentFrame++;
        }
    } 
}
