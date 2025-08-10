using ECS;
using ECSGame;
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

    public static class AppStatic
    {
        public static GameType GameType { get; set; } = GameType.ECSGame;
    }
}