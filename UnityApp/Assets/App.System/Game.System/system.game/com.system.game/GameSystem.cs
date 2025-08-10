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
        IUpdate<Game>,
        IFixedUpdate<Game>
    {
        public static Game Create(Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<Game>(EcsType.Game, systemAssembly);
            return game;
        }

        public void Init(Game game)
        {

        }

        public void Update(Game entity)
        {
            if (entity.Type == ((int)GameType.TrueGameDemo))
            {
                GameTrueWorldSystem.Update(entity);
            }
            else
            {
                GameWorldSystem.Update(entity);
            }
        }

        public void FixedUpdate(Game entity)
        {
            if (entity.Type == ((int)GameType.TrueGameDemo))
            {
                GameTrueWorldSystem.FixedUpdate(entity);
            }
            else
            {
                GameWorldSystem.FixedUpdate(entity);
            }
        }
    }
}
