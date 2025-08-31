using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 高度内聚可以复用的系统，做成可导入的第三方模块
    /// 与游戏业务强耦合的顶层系统，做成可定制的本地化模块，比如Player、Actor、Game等
    /// </summary>
    public class Game : EcsNode
    {
        public Game(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
        public int Type { get; set; }
    }
}
