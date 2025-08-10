using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameWorldSystem : AEntitySystem<GameWorld>,
    IInit<GameWorld>,
    IUpdate<GameWorld>
    {
        public static GameWorld Create(ushort nodeIndex, Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<GameWorld>(nodeIndex, systemAssembly);
            return game;
        }

        public void Init(GameWorld game)
        {

        }

        public void Update(GameWorld game)
        {

        }
    }
}
