using ECS;
using ECS.Fody;
using ECSUnity;
using Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class TrueGameUISystem : AEntitySystem<TrueGame>,
        IInit<TrueGame>,
        IOnChange<TrueGame>
    {
        public void Init(TrueGame game)
        {
        }

        public void OnChange(TrueGame game)
        {

        }

        [After(typeof(TrueGameSystem), nameof(TrueGameSystem.FrameSeal))]
        public static void OnFrameSeal(TrueGame game)
        {
            var homePageWindow = UISystem.GetWindow<UI_HomePageWindow>();
            if (homePageWindow == null)
            {
                return;
            }
            homePageWindow.GameUIRefill(game);
        }
    }
}
