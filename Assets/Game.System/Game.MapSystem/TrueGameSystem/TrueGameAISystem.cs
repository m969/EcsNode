using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
    public class TrueGameAISystem
    {
        public static void FrameUpdate(TrueGame game, long determineFrame)
        {
            var frame = determineFrame;

            var allEntities = game.Id2Children.Values.ToArray();

            foreach ( var entity in allEntities)
            {
                if (entity.IsDispose) continue;
                if (entity.GetComponent<AIComponent>() is { } component)
                {
                    AISystem.FrameUpdate(entity, component);
                }
            }
        }
    }
}
