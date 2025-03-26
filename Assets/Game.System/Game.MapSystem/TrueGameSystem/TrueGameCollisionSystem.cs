using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
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

        public static void FrameUpdate(TrueGame game, TrueGameCollisionComponent component)
        {
            var frame = game.CurrentFrame;

            var allEntities = game.Id2Children.Values.ToArray();

            foreach ( var entity in allEntities)
            {
                if (entity.IsDispose) continue;
                if (entity is Item item)
                {
                    var pos = item.GetComponent<TransformComponent>().Position;
                    if (FP.Abs(pos.x) > 10 || FP.Abs(pos.z) > 10)
                    {
                        EcsObject.Destroy(entity);
                    }
                }
            }
        }
    }
}
