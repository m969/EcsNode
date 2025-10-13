using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrueSync;

namespace ECSUnity
{
    public class DomainViewSystem
    {
        public static UIStage AddUI(Assembly systemAssembly)
        {
            var uiStage = UISystem.Create(systemAssembly);
            uiStage.Init();
            EcsDomain.AddNode(uiStage);
            EcsDomain.UIStage = uiStage;
            return uiStage;
        }

        public static SoundMaster AddSound(Assembly systemAssembly)
        {
            var soundMaster = SoundSystem.Create(systemAssembly);
            soundMaster.Init();
            EcsDomain.AddNode(soundMaster);
            EcsDomain.SoundMaster = soundMaster;
            return soundMaster;
        }

        public static PlayerInput AddPlayerInput(Assembly systemAssembly)
        {
            var playerInput = PlayerInputSystem.Create(systemAssembly);
            EcsDomain.AddNode(playerInput);
            EcsDomain.PlayerInput = playerInput;
            return playerInput;
        }
    }
}
