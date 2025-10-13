using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class GamePlayerInputComponent : EcsComponent
    {
        public PlayerInput PlayerInput { get; set; }
    }
}
