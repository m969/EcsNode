using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSUnity
{
    public class UnityApp : EcsNode
    {
        public UnityApp(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}
