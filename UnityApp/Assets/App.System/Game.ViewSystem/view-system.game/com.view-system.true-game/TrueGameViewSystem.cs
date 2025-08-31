using ECS;
using ECSGame;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    [SystemGameFilter((int)GameType.TrueGameDemo)]
    public class TrueGameViewSystem : AEntitySystem<Game>,
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
            UISystem.Show<LoginUI.UI_HomePageWindow>();
        }
    }
}