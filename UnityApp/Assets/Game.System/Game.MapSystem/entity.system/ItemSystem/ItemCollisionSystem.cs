using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ItemCollisionSystem : AEntitySystem<Item>, ICollisionHandler
    {
        public void OnCollision(EcsEntity self, EcsEntity other)
        {
            EcsObject.Destroy(self);
        }
    }
}
