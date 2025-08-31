using ECS;
using ECSGame;
using FairyGUI;
using LoginUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    [SystemGameFilter((int)GameType.SimulationGameDemo)]
    public class SimulationGameViewSystem : AEntitySystem<Game>,
    IInit<Game>,
    IAfterInit<Game>
    {
        public void Init(Game game)
        {

        }

        public void AfterInit(Game game)
        {

        }

        public static void ReloadUI()
        {
            UISystem.Show<GameUI.UI_GameWindow>();
        }
    }
}