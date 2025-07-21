using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class HealthComponent : EcsComponent
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
    }
}