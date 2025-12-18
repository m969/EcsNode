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
        // IEventHandle<InputEvent>
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

        // public void OnHandleEvent(EcsNode ecsNode, InputEvent eventContext)
        // {
        //     ConsoleLog.Debug("OnHandleEvent: InputEvent " + eventContext.InputType);
        //     if (ecsNode.TryGetComponent<TrueGameInputComponent>(out var component) == false)
        //     {
        //         return;
        //     }

        //     var myActor = UnityAppStatic.MyActor;
        //     var advanceFrame = EcsDomain.TrueWorld.DetermineFrame + TrueWorld.ForecastFrame;

        //     if (eventContext.InputType == InputType.Fire)
        //     {
        //         var input = new InputData()
        //         {
        //             Frame = advanceFrame,
        //             PlayerId = myActor.Id,
        //             InputType = InputType.Fire,
        //             InputVector = eventContext.Direction.ToTSVector(),
        //         };

        //         ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
        //     }
        // }
    }
}
