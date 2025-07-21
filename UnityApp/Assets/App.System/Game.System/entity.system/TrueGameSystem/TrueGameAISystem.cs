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
            var allEntities = game.Id2Children.Values.ToArray();
            foreach ( var entity in allEntities)
            {
                if (entity.IsDisposed) continue;
                if (entity.GetComponent<AIComponent>() is { Enable:true } component)
                {
                    AISystem.FrameUpdate(entity, component, determineFrame);
                }
            }
        }
    }
}
