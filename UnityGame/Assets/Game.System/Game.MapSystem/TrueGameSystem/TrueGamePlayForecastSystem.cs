using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;

namespace ECSGame
{
    public class TrueGamePlayForecastSystem : AComponentSystem<TrueGame, TrueGamePlayForecastComponent>,
IAwake<TrueGame, TrueGamePlayForecastComponent>,
IInit<TrueGame, TrueGamePlayForecastComponent>
    {
        public void Awake(TrueGame game, TrueGamePlayForecastComponent component)
        {
        }

        public void Init(TrueGame game, TrueGamePlayForecastComponent component)
        {
        }

        public static void FrameUpdate(TrueGame game, TrueGamePlayForecastComponent component, long determineFrame)
        {
            var actors = game.Id2Children.Values.ToArray();
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
