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
    public class UIShowWindowEventHandler : AEventRun<UIStage, UIShowWindowEvent>
    {
        private IUIWindow Show(Type type, Action<IUIWindow> beforeAwake = null)
        {
            EcsDomain.UIStage.Type2Windows.TryGetValue(type, out var window);
            if (window == null)
            {
                var uiobject = (IUIWindow)UIPackage.CreateObjectFromURL(UIObjectFactory.packageType2Items[type]);
                var uipanel = uiobject as UIPanel;
                uipanel.SetSize(GRoot.inst.width, GRoot.inst.height);
                //当屏幕改变时仍然保持全屏。如果屏幕大小始终不变，则这句也可以忽略
                uipanel.AddRelation(GRoot.inst, RelationType.Size);
                beforeAwake?.Invoke(uiobject);
                uiobject.Awake();
                GRoot.inst.AddChild(uipanel);
                EcsDomain.UIStage.Type2Windows.Add(type, uipanel);
                uipanel.Show();
                return uiobject;
            }
            else
            {
                var uiobject = (IUIWindow)window;
                var uipanel = uiobject as UIPanel;
                uipanel.Show();
                return uiobject;
            }
        }

        protected override async ETTask Run(UIStage uiStage, UIShowWindowEvent showWindowEvent)
        {
            Show(showWindowEvent.WindowType, showWindowEvent.BeforeAwake);
        }
    }
}
