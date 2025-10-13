using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
    public class TrueWorldAISystem
    {
        public static void FrameUpdate(TrueWorld world, long determineFrame)
        {
            var allEntities = world.Id2Children.Values.ToArray();
            foreach (var entity in allEntities)
            {
                if (entity.IsDisposed) continue;
                if (entity.GetComponent<AIComponent>() is { Enable: true } component)
                {
                    AISystem.Update(entity, component, determineFrame);
                }
            }
        }
    }
}
