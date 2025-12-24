using ECS;
using ECSGame;
using ECSGame.UnitDispatchModule;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public enum GameType
    {
        ECSGame,
        TrueGameDemo,
        SimulationGameDemo,
    }

    public class EcsType
    {
        /// <summary>
        /// Unity应用
        /// </summary>
        public const ushort UnityApp = 1;

        /// <summary>
        /// UI
        /// </summary>
        public const ushort UI = 2;

        /// <summary>
        /// 声音
        /// </summary>
        public const ushort Sound = 3;

        /// <summary>
        /// 玩家输入
        /// </summary>
        public const ushort PlayerInput = 4;

        /// <summary>
        /// 帧同步地图世界
        /// </summary>
        public const ushort TrueWorld = 5;

        /// <summary>
        /// 游戏地图世界
        /// </summary>
        public const ushort World = 6;

        /// <summary>
        /// 玩家实体
        /// </summary>
        public const ushort Player = 7;
        
        /// <summary>
        /// 游戏主流程
        /// </summary>
        public const ushort Game = 8;
    }

    public static class AppStatic
    {
        public static long NowMilliseconds { get; set; }
        public static float NowSeconds { get; set; }
        public static long DeltaTimeMilliseconds { get; set; }
        public static float DeltaTimeSeconds { get; set; }
        public static Game Game { get; set; }
        public static Player Player { get; set; }
        public static GameType GameType { get; set; } = GameType.ECSGame;
        public static Actor MyActor { get; set; }
        public static Actor OtherActor { get; set; }
    }
}