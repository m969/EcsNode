using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class ActorDeathEvent : IEvent
    {
        public Actor Actor { get; set; }
    }
}
