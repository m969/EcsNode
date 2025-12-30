using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GamePlayerSystem : AComponentSystem<Game, GamePlayerComponent>,
    IInit<Game, GamePlayerComponent>
    {
        public void Init(Game game, GamePlayerComponent component)
        {
            var systemAssembly = game.GetComponent<ReloadComponent>().SystemAssembly;
            var player = PlayerSystem.Create(systemAssembly);
            player.Init();
            component.Player = player;
            AppStatic.Player = player;
        }
    }
}
