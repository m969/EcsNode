using ECS;
using ECSGame;
using FairyGUI;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    public class GamePlayerInputSystem : AComponentSystem<Game, GamePlayerInputComponent>,
    IInit<Game, GamePlayerInputComponent>,
    IAfterInit<Game, GamePlayerInputComponent>
    {
        public void Init(Game game, GamePlayerInputComponent component)
        {

        }

        public void AfterInit(Game game, GamePlayerInputComponent component)
        {
            var systemAssembly = game.GetComponent<ReloadComponent>().SystemAssembly;
            var playerInput = DomainViewSystem.AddPlayerInput(systemAssembly);
            if (game.Type == (int)GameType.TrueGameDemo)
            {
                playerInput.AddComponent<TrueGameInputComponent>();
            }
            if (game.Type == (int)GameType.SimulationGameDemo)
            {
                playerInput.AddComponent<SimulationGameInputComponent>();
            }
            playerInput.Init();
        }

        public static void Update(Game entity)
        {
            EcsDomain.PlayerInput?.DriveEntityUpdate();
        }
    }
}