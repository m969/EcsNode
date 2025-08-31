using ECS;
using ECS.Fody;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class TrueWorldUISystem : AEntitySystem<TrueWorld>,
        IInit<TrueWorld>,
        IOnChange<TrueWorld>
    {
        public void Init(TrueWorld game)
        {
        }

        public void OnChange(TrueWorld game)
        {

        }

        [After(typeof(TrueWorldSystem), nameof(TrueWorldSystem.FrameSeal))]
        public static void OnFrameSeal(TrueWorld game)
        {
            var homePageWindow = UISystem.GetWindow<LoginUI.UI_HomePageWindow>();
            if (homePageWindow == null)
            {
                return;
            }
            homePageWindow.GameUIRefill(game);
        }
    }
}
