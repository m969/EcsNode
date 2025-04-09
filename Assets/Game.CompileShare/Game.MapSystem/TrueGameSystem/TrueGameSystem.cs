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
            game.AddComponent<TrueGamePlayForecastComponent>();
            return game;
        }

        public void Init(TrueGame game)
        {
            game.TSRandom = new TrueSync.TSRandom(1);
            game.StartFrameTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 封帧
        /// </summary>
        public static void FrameSeal(TrueGame game)
        {
            // 确定帧更新
            game.DetermineFrame = game.NextFrame;
            game.NextFrame++;
        }

        // 帧同步核心逻辑
        public void Update(TrueGame game)
        {
            var currentFrame = game.NextFrame;

            // 计算下一帧理论执行时间
            long nextFrameTime = currentFrame * game.FrameInterval + game.StartFrameTime;

            // 等待到下一帧时间
            while ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) < nextFrameTime)
            {
                return;
            }

            var determineFrame = currentFrame;
            FrameSeal(game);

            // 处理确定帧逻辑
            
            if (game.GetComponent<TrueGameCollisionComponent>() is { } component1)
            {
                // 碰撞检测处理
                TrueGameCollisionSystem.FrameUpdate(game, component1, determineFrame);
            }

            {
                // AI逻辑处理
                TrueGameAISystem.FrameUpdate(game, determineFrame);
            }

            //if (game.GetComponent<TrueGameExecuteComponent>() is { } component2)
            //{
            //    // 执行玩家输入
            //    TrueGameExecuteSystem.FrameUpdate(game, component2, determineFrame);
            //}

            game.OnFrameUpdate?.Invoke(game, determineFrame);

            if (game.GetComponent<TrueGamePlayComponent>() is { } component3)
            {
                // 模拟播放运行
                TrueGamePlaySystem.FrameUpdate(game, component3, determineFrame);
            }

            if (game.GetComponent<TrueGamePlayForecastComponent>() is { } component4)
            {
                // 模拟预测运行
                TrueGamePlayForecastSystem.FrameUpdate(game, component4, determineFrame);
            }
        }
    } 
}
