using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class GameWorld : EcsNode
    {
        public Actor PlayerActor { get; set; }

        public GameWorld(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}
