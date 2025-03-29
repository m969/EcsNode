using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TrueSync;

namespace ECSGame
{
    // 输入指令结构
    public struct PlayerInput
    {
        public long Frame;
        public long PlayerId;
        public InputType InputType;
        public TSVector InputVector;
    }

    public enum InputType
    {
        None = 0,
        Move = 10,
        StopMove = 20,
        Look = 30,
        Fire = 40,
        StopFire = 50,
    }

    // 帧执行命令
    public interface IFramePlay
    {
        public long EntityId { get; set; }
    }

    public struct FramePlay_Move : IFramePlay
    {
        public long EntityId { get; set; }
        public TSVector Position;
        public TSVector AfterPosition;
    }

    public struct FramePlay_Fire : IFramePlay
    {
        public long EntityId { get; set; }
        public TSVector Direction;
    }

    public enum StatePlayType
    {
        None = 0,
        Move = 10,
        Fire = 20,
    }

    public class TrueGame : EcsEntity
    {
        // 游戏逻辑帧率（每秒帧数）
        public const int FPS = 20;

        public const int ForecastFrame = FPS / 10 + 1;

        // 当前逻辑帧编号
        public long DetermineFrame;

        public long StartFrameTime;

        // 当前输入帧编号
        public long CurrentInputFrame;

        // 逻辑帧间隔（毫秒）
        public readonly long FrameInterval = 1000 / FPS;

        public TSRandom TSRandom;

        public Actor MyActor { get; set; }
    } 
}
