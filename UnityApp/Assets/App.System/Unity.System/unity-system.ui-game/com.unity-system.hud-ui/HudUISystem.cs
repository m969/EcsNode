using ECS;
using ECSGame;
using ECS.Fody;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    public class HudUISystem : AComponentSystem<Actor, HudUIComponent>,
        IInit<Actor, HudUIComponent>,
        IOnChange<Actor, HudUIComponent>
    {
        public void Init(Actor actor, HudUIComponent component)
        {
        }

        public void OnChange(Actor actor, HudUIComponent component)
        {

        }
    }
}
