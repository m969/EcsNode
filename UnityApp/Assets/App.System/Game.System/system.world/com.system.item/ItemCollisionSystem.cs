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
            var eventContext = new CollisionEvent{
                Self = self,
                Other = other
            };
            EventBus.Send(eventContext);
            EcsObject.Destroy(self);
        }
    }
}
