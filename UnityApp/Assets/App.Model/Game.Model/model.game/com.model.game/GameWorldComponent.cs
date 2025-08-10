using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class GameWorldComponent : EcsComponent
    {
        public World World { get; set; }
    }
}
