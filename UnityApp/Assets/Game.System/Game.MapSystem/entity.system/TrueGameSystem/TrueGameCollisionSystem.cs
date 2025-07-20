using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
    public interface ICollisionHandler : IDispatch
    {
        void OnCollision(EcsEntity self, EcsEntity other);
    }

    public class TrueGameCollisionSystem : AComponentSystem<TrueGame, TrueGameCollisionComponent>,
        IAwake<TrueGame, TrueGameCollisionComponent>,
        IInit<TrueGame, TrueGameCollisionComponent>
    {
        public void Awake(TrueGame game, TrueGameCollisionComponent component)
        {
        }

        public void Init(TrueGame game, TrueGameCollisionComponent component)
        {
        }

        public static void FrameUpdate(TrueGame game, TrueGameCollisionComponent component, long determineFrame)
        {
            var frame = determineFrame;

            var allEntities = game.Id2Children.Values.ToArray();

            foreach (var entity in allEntities)
            {
                if (entity.IsDisposed) continue;
                var collision1 = entity.GetComponent<CollisionComponent>();
                if (collision1 == null) continue;
                foreach (var entity2 in allEntities)
                {
                    if (entity2.IsDisposed) continue;
                    if (entity == entity2) continue;
                    if (entity is Item && entity2 is Item) continue;
                    var collision2 = entity2.GetComponent<CollisionComponent>();
                    if (collision2 == null) continue;
                    if (collision1.Layer == collision2.Layer) continue;

                    var dist = TSVector.Distance(TransformSystem.GetPosition(entity), TransformSystem.GetPosition(entity2));
                    if (dist < 2)
                    {
                        entity.Dispatch<ICollisionHandler>(handler => handler.OnCollision(entity, entity2));
                        entity2.Dispatch<ICollisionHandler>(handler => handler.OnCollision(entity2, entity));
                    }
                }

                if (entity is Item item)
                {
                    var pos = TransformSystem.GetPosition(item);
                    if (FP.Abs(pos.x) > 10 || FP.Abs(pos.z) > 10)
                    {
                        entity.Dispatch<ICollisionHandler>(handler => handler.OnCollision(entity, entity));
                    }
                }
            }
        }
    }
}
