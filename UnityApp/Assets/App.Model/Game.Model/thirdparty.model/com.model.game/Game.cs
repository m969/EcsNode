using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TrueSync;

namespace ECSGame
{
    public class Game : EcsNode
    {
        public Game(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
        public int Type { get; set; }
    }
}
