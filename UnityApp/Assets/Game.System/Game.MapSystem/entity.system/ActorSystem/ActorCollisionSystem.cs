using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorCollisionSystem : AEntitySystem<Actor>, ICollisionHandler
    {
        public void OnCollision(EcsEntity self, EcsEntity other)
        {

        }
    }
}
