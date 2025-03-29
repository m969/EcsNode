using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
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
            //ConsoleLog.Debug("TrueGamePlayForecastSystem FrameUpdate");
            var actors = game.Id2Children.Values;
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


            //var frame = game.CurrentFrame;
            //var forecastFrame = frame + TrueGame.ForecastFrame;
            //var playComp = game.GetComponent<TrueGamePlayComponent>();

            //if (!component.FramePlays.ContainsKey(frame))
            //{
            //    var framePlays = new List<IFramePlay>();
            //    framePlays.AddRange(playComp.FramePlays[frame]);
            //    component.FramePlays[frame] = framePlays;
            //}
            //else
            //{
            //    var framePlays = component.FramePlays[frame];
            //    var trueFramePlays = playComp.FramePlays[frame];
            //}

            //for (var i = frame; i <= forecastFrame; i++)
            //{
            //    if (!component.FramePlays.ContainsKey(forecastFrame))
            //    {
            //        component.FramePlays[forecastFrame] = new List<IFramePlay>();
            //    }
            //}
        }
    }
}
