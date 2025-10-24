using ECS;
using ECSGame;
using System;
using System.Reflection;

namespace ECSUnity
{
    public class UnityInputSystem : AComponentSystem<UnityApp, UnityInputComponent>,
    IAwake<UnityApp, UnityInputComponent>
    {
        public void Awake(UnityApp app, UnityInputComponent component)
        {
            var systemAssembly = app.GetComponent<ReloadComponent>().SystemAssembly;
            var playerInput = PlayerInputSystem.Create(systemAssembly);
            playerInput.Init();
            EcsDomain.AddNode(playerInput);
            EcsDomain.PlayerInput = playerInput;
        }
    }
}