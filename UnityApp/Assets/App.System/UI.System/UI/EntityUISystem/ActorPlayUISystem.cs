using ECS;
using ECS.Fody;
using ECSUnity;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class ActorPlayUISystem : AComponentSystem<Actor, FramePlayComponent>,
        IInit<Actor, FramePlayComponent>,
        IOnChange<Actor, FramePlayComponent>
    {
        public void Init(Actor actor, FramePlayComponent component)
        {
        }

        public void OnChange(Actor actor, FramePlayComponent component)
        {

        }

        [After(typeof(ActorPlaySystem), nameof(ActorPlaySystem.FrameUpdate))]
        public static void OnFrameUpdate(Actor actor, FramePlayComponent component, long determineFrame)
        {
            var homePageWindow = UISystem.GetWindow<UI_HomePageWindow>();
            if (homePageWindow == null)
            {
                return;
            }
            if (StaticObject.OtherActor == actor)
            {
                homePageWindow.ActorUIRefill(actor);
            }
        }
    }
}
