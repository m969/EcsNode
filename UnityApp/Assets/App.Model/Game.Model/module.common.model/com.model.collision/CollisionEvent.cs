using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class CollisionEvent : IEvent
    {
        public EcsEntity Self { get; set; }
        public EcsEntity Other { get; set; }
    }
}
