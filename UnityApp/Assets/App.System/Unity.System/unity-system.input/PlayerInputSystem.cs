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
        IUpdate<PlayerInput>,
        IEventDispatch<InputEvent>
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

        public void OnHandleEvent(EcsNode ecsNode, InputEvent inputEvent)
        {
            ConsoleLog.Debug("OnHandleEvent: InputEvent " + inputEvent.InputType);
            if (ecsNode.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }

            var myActor = component.PlayerActor;
            var advanceFrame = component.TrueWorld.DetermineFrame + TrueWorld.ForecastFrame;

            if (inputEvent.InputType == InputType.Fire)
            {
                component.FireVector = inputEvent.Direction;
                var input = new InputData()
                {
                    Frame = advanceFrame,
                    PlayerId = myActor.Id,
                    InputType = InputType.Fire,
                    InputVector = inputEvent.Direction.ToTSVector(),
                };

                ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
