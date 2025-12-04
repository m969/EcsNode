using ECS;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Threading.Tasks;
using System.Reflection;
using ECSGame;

namespace ECSUnity
{
    public partial class UISystem : AEntitySystem<UIStage>,
        IInit<UIStage>
    {
        public void Init(UIStage entity)
        {

        }

        public static UIStage Create(Assembly systemAssembly)
        {
            var ecsNode = EcsNodeSystem.Create<UIStage>(EcsType.UI, systemAssembly);
            return ecsNode;
        }

        public static async ETTask WaitDelay(EcsNode entity, long time)
        {
            //ConsoleLog.Debug("UISystem WaitDelay");
            await TimerSystem.WaitAsync(time);
            //ConsoleLog.Debug("UISystem WaitDelay2");
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

        public static async ETTask<T> ShowAsync<T>(Action<T> beforeAwake = null) where T : UIPanel, IUIWindow
        {
            var type = typeof(T);

            var showEvent = new UIShowWindowEvent()
            {
                WindowType = type,
                BeforeAwake = (Action<IUIWindow>)beforeAwake,
                CompleteTask = ETTask.Create()
            };
            EventBus.Send(showEvent);
            await showEvent.CompleteTask;

            EcsDomain.UIStage.Type2Windows.TryGetValue(type, out var window);
            if (window == null)
            {
                return null;
            }
            var uiobject = (T)window;
            return uiobject;
        }

        public static void Show<T>(Action<T> beforeAwake = null) where T : UIPanel, IUIWindow
        {
            var type = typeof(T);
            var showEvent = new UIShowWindowEvent()
            {
                WindowType = type,
                BeforeAwake = (Action<IUIWindow>)beforeAwake,
                CompleteTask = ETTask.Create()
            };
            EventBus.Send(showEvent);
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
