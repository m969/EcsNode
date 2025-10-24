using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSUnity
{
    public class UnityInputComponent : EcsComponent
    {
        public PlayerInput PlayerInput { get; set; }
    }
}
