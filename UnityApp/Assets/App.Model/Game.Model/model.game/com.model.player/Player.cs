using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class Player : EcsNode
    {
        public Player(ushort ecsTypeId) : base(ecsTypeId)
        {
        }

        public int Type { get; set; }

        public long ActorId { get; set; }
    }
}
