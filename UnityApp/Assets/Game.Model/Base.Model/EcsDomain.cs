using ECS;
using ECSGame;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public class EcsType
    {
        /// <summary>
        /// 游戏主流程
        /// </summary>
        public const ushort Game = 1;

        /// <summary>
        /// UI
        /// </summary>
        public const ushort UI = 2;

        /// <summary>
        /// 声音
        /// </summary>
        public const ushort Sound = 3;
    }

    public static class EcsDomain
    {
        public readonly static Dictionary<ushort, EcsNode> EcsNodes = new();

        public static TrueGame Game { get; set; }
        public static UIStage UIStage { get; set; }
        public static SoundMaster SoundMaster { get; set; }


        public static void AddNode(EcsNode node)
        {
            EcsNodes.Add(node.EcsTypeId, node);
        }

        public static EcsNode GetNode(ushort typeId)
        {
            EcsNodes.TryGetValue(typeId, out var ecsNode);
            return ecsNode;
        }
    }
}