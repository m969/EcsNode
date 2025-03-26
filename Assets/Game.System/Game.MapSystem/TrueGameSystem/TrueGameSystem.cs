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

        public static TrueGame Create(EcsNode ecsNode)
        {
            var game = ecsNode.AddChild<TrueGame>();
            game.AddComponent<TrueGameExecuteComponent>();
            game.AddComponent<TrueGamePlayComponent>();
            game.AddComponent<TrueGameCollisionComponent>();
            return game;
        }

        public void Init(TrueGame game)
        {
            game.TSRandom = new TrueSync.TSRandom(1);
            game.StartFrameTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        // 帧同步核心逻辑
        public void Update(TrueGame game)
        {
            // 计算下一帧理论执行时间
            long nextFrameTime = game.CurrentFrame * game.FrameInterval + game.StartFrameTime;

            // 等待到下一帧时间
            while ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) < nextFrameTime)
            {
                return;
            }

            // 处理当前帧逻辑

            if (game.GetComponent<TrueGameCollisionComponent>() is { } component1)
            {
                // 碰撞检测处理
                TrueGameCollisionSystem.FrameUpdate(game, component1);
            }

            // AI逻辑处理
            TrueGameAISystem.FrameUpdate(game);

            if (game.GetComponent<TrueGameExecuteComponent>() is { } component2)
            {
                // 执行玩家输入
                TrueGameExecuteSystem.FrameUpdate(game, component2);
            }

            if (game.GetComponent<TrueGamePlayComponent>() is { } component3)
            {
                // 模拟播放游戏运行
                TrueGamePlaySystem.FrameUpdate(game, component3);
            }

            // 推进帧数
            game.CurrentFrame++;
        }
    } 
}
