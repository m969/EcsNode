using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameSystem : AEntitySystem<Game>,
    IInit<Game>,
    IUpdate<Game>
    {
        public static Game Create(Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<Game>(EcsType.Game, systemAssembly);
            return game;
        }

        public void Init(Game game)
        {

        }

        public void Update(Game game)
        {

        }
    }
}
