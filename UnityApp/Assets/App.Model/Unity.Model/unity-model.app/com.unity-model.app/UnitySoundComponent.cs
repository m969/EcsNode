using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSUnity
{
    public class UnitySoundComponent : EcsComponent
    {
        public SoundMaster SoundMaster { get; set; }
    }
}
