using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameViewSystem : AEntitySystem<Game>,
        IInit<Game>,
        IUpdate<Game>
    {
        public void Init(Game game)
        {

        }

        public void Update(Game entity)
        {
            // GamePlayerInputSystem.Update(entity);
        }
    }
}
