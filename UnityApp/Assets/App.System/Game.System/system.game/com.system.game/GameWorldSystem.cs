using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameWorldSystem : AComponentSystem<Game, GameWorldComponent>,
    IInit<Game, GameWorldComponent>
    {
        public void Init(Game game, GameWorldComponent component)
        {
            var systemAssembly = game.GetComponent<ReloadComponent>().SystemAssembly;
            var gameWorld = WorldSystem.Create(systemAssembly);
            gameWorld.Init();
            component.World = gameWorld;
        }

        public static void Update(Game game)
        {
            if (game.TryGetComponent<GameWorldComponent>(out var component) == false)
            {
                return;
            }
            if (component.World == null)
            {
                return;
            }
            component.World.DriveEntityUpdate();
        }

        public static void FixedUpdate(Game game)
        {
            if (game.TryGetComponent<GameWorldComponent>(out var component) == false)
            {
                return;
            }
            if (component.World == null)
            {
                return;
            }
            component.World.DriveEntityFixedUpdate();
        }
    
        public static World GetWorld(Game game)
        {
            if (game.TryGetComponent<GameWorldComponent>(out var component) == false)
            {
                return null;
            }
            return component.World;
        }
    }
}
