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
    [SystemPlatformFilter((int)RuntimePlatform.Android)]
    [SystemPlatformFilter((int)RuntimePlatform.IPhonePlayer)]
    [SystemPlatformFilter((int)RuntimePlatform.WindowsPlayer)]
    [SystemPlatformFilter((int)RuntimePlatform.OSXPlayer)]
    [SystemGameFilter((int)GameType.SimulationGameDemo)]
    [SystemEcsFilter(EcsType.PlayerInput)]
    public class SimulationGameInputSystem : AComponentSystem<PlayerInput, SimulationGameInputComponent>,
        IAwake<PlayerInput, SimulationGameInputComponent>,
        IInit<PlayerInput, SimulationGameInputComponent>
    {
        public void Awake(PlayerInput playerInput, SimulationGameInputComponent component)
        {
            //component.PlayerActor = StaticObject.MyActor;
        }

        public void Init(PlayerInput playerInput, SimulationGameInputComponent component)
        {
        }

        public static void Update(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<SimulationGameInputComponent>(out var component) == false)
            {
                return;
            }
        }
    }
}
