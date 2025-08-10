using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class GamePlayerComponent : EcsComponent
    {
        public Player Player { get; set; }
    }
}
