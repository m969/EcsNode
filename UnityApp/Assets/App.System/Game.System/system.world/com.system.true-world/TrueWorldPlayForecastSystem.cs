using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
    public class TrueWorldPlayForecastSystem : AComponentSystem<TrueWorld, TrueWorldPlayForecastComponent>,
IAwake<TrueWorld, TrueWorldPlayForecastComponent>,
IInit<TrueWorld, TrueWorldPlayForecastComponent>
    {
        public void Awake(TrueWorld game, TrueWorldPlayForecastComponent component)
        {
        }

        public void Init(TrueWorld game, TrueWorldPlayForecastComponent component)
        {
        }

        public static void FrameUpdate(TrueWorld trueWorld, long determineFrame)
        {
            if (!trueWorld.TryGetComponent<TrueWorldPlayForecastComponent>(out var component))
            {
                return;
            }

            var actors = trueWorld.Id2Children.Values.ToArray();
            foreach (var entity in actors)
            {
                if (entity is Actor actor)
                {
                    if (actor.GetComponent<FramePlayComponent>() is { } component2)
                    {
                        ActorPlaySystem.FrameUpdate(actor, component2, determineFrame);
                    }
                }
            }
        }
    }
}
