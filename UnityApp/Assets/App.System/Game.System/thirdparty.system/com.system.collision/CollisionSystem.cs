using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;
using static UnityEngine.EventSystems.EventTrigger;

namespace ECSGame
{
    public class CollisionSystem : AComponentSystem<EcsEntity, CollisionComponent>,
IAwake<EcsEntity, CollisionComponent>
    {
        public void Awake(EcsEntity entity, CollisionComponent component)
        {
        }

        public static void SetLayer(EcsEntity entity, uint layer)
        {
            if (entity.IsDisposed) return;
            var collisionComponent = entity.GetComponent<CollisionComponent>();
            if (collisionComponent != null)
            {
                collisionComponent.Layer = layer;
            }
        }
    }
}
