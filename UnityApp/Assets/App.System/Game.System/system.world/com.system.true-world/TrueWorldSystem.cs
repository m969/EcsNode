using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public interface IOnFrameUpdate : IDispatch
    {
        void OnFrameUpdate(TrueWorld trueWorld, long determineFrame);
    }

    public class TrueWorldSystem : AEntitySystem<TrueWorld>,
    IInit<TrueWorld>,
    IUpdate<TrueWorld>
    {
        public static TrueWorld Create(Assembly systemAssembly)
        {
            var trueWorld = EcsNodeSystem.Create<TrueWorld>(EcsType.TrueWorld, systemAssembly);
            trueWorld.AddComponent<TrueWorldPlayComponent>();
            trueWorld.AddComponent<TrueWorldCollisionComponent>();
            trueWorld.AddComponent<TrueWorldPlayForecastComponent>();
            return trueWorld;
        }

        public void Init(TrueWorld trueWorld)
        {
            trueWorld.TSRandom = new TrueSync.TSRandom(1);
            trueWorld.StartFrameTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 封帧
        /// </summary>
        public static void FrameSeal(TrueWorld trueWorld)
        {
            // 确定帧更新
            trueWorld.DetermineFrame = trueWorld.NextFrame;
            trueWorld.NextFrame++;
        }

        // 帧同步核心逻辑
        public void Update(TrueWorld trueWorld)
        {
            var currentFrame = trueWorld.NextFrame;
            // 计算下一帧理论执行时间
            long nextFrameTime = currentFrame * trueWorld.FrameInterval + trueWorld.StartFrameTime;

            // 等待到下一帧时间
            while ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) < nextFrameTime)
            {
                return;
            }

            var determineFrame = currentFrame;
            FrameSeal(trueWorld);

            // 处理确定帧逻辑

            // 碰撞检测处理
            TrueWorldCollisionSystem.FrameUpdate(trueWorld, determineFrame);

            // AI逻辑处理
            TrueWorldAISystem.FrameUpdate(trueWorld, determineFrame);

            trueWorld.Dispatch<IOnFrameUpdate>(system => system.OnFrameUpdate(trueWorld, determineFrame));

            // 模拟播放运行
            TrueWorldPlaySystem.FrameUpdate(trueWorld, determineFrame);

            // 模拟预测运行
            TrueWorldPlayForecastSystem.FrameUpdate(trueWorld, determineFrame);
        }
    }
}
