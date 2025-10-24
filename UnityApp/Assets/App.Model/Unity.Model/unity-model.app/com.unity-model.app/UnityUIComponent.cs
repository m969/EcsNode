using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSUnity
{
    public class UnityUIComponent : EcsComponent
    {
        public UIStage UIStage { get; set; }
    }
}
