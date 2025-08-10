using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class GameTrueWorldComponent : EcsComponent
    {
        public TrueWorld World { get; set; }
    }
}
