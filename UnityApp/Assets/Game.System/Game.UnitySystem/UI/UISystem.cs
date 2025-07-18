using ECS;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Login;
using System.Threading.Tasks;
using System.Reflection;
using ECSGame;

namespace ECSUnity
{
    public class UISystem : AEntitySystem<UIStage>,
IInit<UIStage>
    {
        public void Init(UIStage entity)
        {

        }

        public static UIStage Create(ushort nodeIndex, Assembly systemAssembly)
        {
            var ecsNode = EcsNodeSystem.Create<UIStage>(nodeIndex, systemAssembly);
            return ecsNode;
        }

        public static async ETTask WaitDelay(EcsNode entity, long time)
        {
            //ConsoleLog.Debug("UISystem WaitDelay");
            await TimerSystem.WaitAsync(entity, time);
            //ConsoleLog.Debug("UISystem WaitDelay2");
        }

        public static void Update(EcsNode entity, UIStage component)
        {
            if (GetWindow<UI_HomePageWindow>() is { } homeWindow)
            {
                homeWindow.Update();
            }
        }

        public static T GetWindow<T>() where T : UIPanel, IUIWindow
        {
            var type = typeof(T);
            EcsDomain.UIStage.Type2Windows.TryGetValue(type, out var window);
            if (window == null)
            {
                return null;
            }
            else
            {
                return (T)window;
            }
        }

        public static T Show<T>(Action<T> beforeAwake = null) where T : UIPanel, IUIWindow
        {
            var type = typeof(T);

            EcsDomain.UIStage.Type2Windows.TryGetValue(type, out var window);
            if (window == null)
            {
                var uiobject = (T)UIPackage.CreateObjectFromURL(UIObjectFactory.packageType2Items[type]);
                beforeAwake?.Invoke(uiobject);
                uiobject.Awake();
                GRoot.inst.AddChild(uiobject);
                EcsDomain.UIStage.Type2Windows.Add(type, uiobject);
                uiobject.Show();
                return uiobject;
            }
            else
            {
                var uiobject = (T)window;
                uiobject.Show();
                return uiobject;
            }
        }

        public static void Hide<T>() where T : UIPanel, IUIWindow
        {
            var type = typeof(T);

            EcsDomain.UIStage.Type2Windows.TryGetValue(type, out var window);
            if (window != null)
            {
                var uiobject = (T)window;
                uiobject.Hide();
            }
        }
    }
}
