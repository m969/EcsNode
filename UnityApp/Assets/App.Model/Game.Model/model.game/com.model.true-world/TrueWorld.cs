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
    public struct InputData
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

        public override bool Equals(object obj)
        {
            var movePlay = (FramePlay_Move)obj;
            //ConsoleLog.Debug($"FramePlay_Move Equals 1 {Position} {AfterPosition}");
            //ConsoleLog.Debug($"FramePlay_Move Equals 2 {movePlay.Position} {movePlay.AfterPosition}");
            return movePlay.EntityId == EntityId && movePlay.Position.Equals(Position) && movePlay.AfterPosition.Equals(AfterPosition);
        }
    }

    public struct FramePlay_MoveStop : IFramePlay
    {
        public long EntityId { get; set; }
        public TSVector Position;
        public TSVector AfterPosition;
        public int LeftStopStep { get; set; }

        public override bool Equals(object obj)
        {
            var movePlay = (FramePlay_MoveStop)obj;
            return movePlay.EntityId == EntityId && movePlay.Position.Equals(Position) && movePlay.AfterPosition.Equals(AfterPosition);
        }
    }

    public struct FramePlay_StopMove : IFramePlay
    {
        public long EntityId { get; set; }
        public TSVector Position;
        public TSVector AfterPosition;
        public int LeftStopStep { get; set; }
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

    //public class TrueGameState<T> : EntityState<T> where T : TrueGame
    //{

    //}

    public class TrueWorld : EcsNode
    {
        //public override IEntityState State { get; set; } = new TrueGameState<TrueGame>();
        public Actor PlayerActor { get; set; }

        // 游戏逻辑帧率（每秒帧数）
        public const int FPS = 20;

        public const int ForecastFrame = FPS / 10 + 1;

        // 最新确定帧编号
        public long DetermineFrame;

        // 下一逻辑帧编号
        public long NextFrame;

        public long StartFrameTime;

        // 当前输入帧编号
        public long CurrentInputFrame;

        // 逻辑帧间隔（毫秒）
        public readonly long FrameInterval = 1000 / FPS;

        public TSRandom TSRandom;

        public Action<TrueWorld, long> OnFrameUpdate { get; set; }


        public TrueWorld(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}
