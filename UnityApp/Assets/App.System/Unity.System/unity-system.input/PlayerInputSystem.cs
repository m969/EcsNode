using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using TrueSync;
using UnityEngine;
using ECSGame;

namespace ECSUnity
{
    public class PlayerInputSystem : AEntitySystem<PlayerInput>,
        IInit<PlayerInput>,
        IUpdate<PlayerInput>
    {
        public void Init(PlayerInput playerInput)
        {

        }

        public void Update(PlayerInput playerInput)
        {
            if (AppStatic.GameType == GameType.TrueGameDemo)
            {
                TrueGameInputSystem.Update(playerInput);
            }
            if (AppStatic.GameType == GameType.SimulationGameDemo)
            {
                SimulationGameInputSystem.Update(playerInput);
            }
        }

        public static PlayerInput Create(Assembly systemAssembly)
        {
            var playerInput = EcsNodeSystem.Create<PlayerInput>(EcsType.PlayerInput, systemAssembly);
            return playerInput;
        }
    }
}
