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
        IEventDispatch<UIShowWindowEvent>
    {
        private IUIWindow Show(UIStage uiStage, Type type, Action<IUIWindow> beforeAwake = null)
        {
            uiStage.Type2Windows.TryGetValue(type, out var window);
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
                uiStage.Type2Windows.Add(type, uipanel);
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

        public void OnHandleEvent(EcsNode ecsNode, UIShowWindowEvent eventContext)
        {
            Show((UIStage)ecsNode, eventContext.WindowType, eventContext.BeforeAwake);
            eventContext.CompleteTask?.SetResult();
        }
    }
}
