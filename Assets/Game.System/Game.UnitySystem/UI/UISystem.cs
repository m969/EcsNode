using ECS;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Login;

namespace ECSUnity
{
    public class UISystem : AComponentSystem<EcsNode, UIComponent>,
IAwake<EcsNode, UIComponent>,
IInit<EcsNode, UIComponent>
    {
        public void Awake(EcsNode entity, UIComponent component)
        {
        }

        public void Init(EcsNode entity, UIComponent component)
        {
        }

        public static void Update(EcsNode entity, UIComponent component)
        {
            if (GetWindow<UI_HomePageWindow>() is { } homeWindow)
            {
                homeWindow.Update();
            }
        }

        public static T GetWindow<T>() where T : UIPanel, IUIWindow
        {
            var ecsNode = StaticObject.EcsNode;
            var uiComp = ecsNode.GetComponent<UIComponent>();
            var type = typeof(T);

            uiComp.Type2Windows.TryGetValue(type, out var window);
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
            var ecsNode = StaticObject.EcsNode;
            var uiComp = ecsNode.GetComponent<UIComponent>();
            var type = typeof(T);

            uiComp.Type2Windows.TryGetValue(type, out var window);
            if (window == null)
            {
                var uiobject = (T)UIPackage.CreateObjectFromURL(UIObjectFactory.packageType2Items[type]);
                beforeAwake?.Invoke(uiobject);
                uiobject.Awake();
                GRoot.inst.AddChild(uiobject);
                uiComp.Type2Windows.Add(type, uiobject);
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
            var ecsNode = StaticObject.EcsNode;
            var uiComp = ecsNode.GetComponent<UIComponent>();
            var type = typeof(T);

            uiComp.Type2Windows.TryGetValue(type, out var window);
            if (window != null)
            {
                var uiobject = (T)window;
                uiobject.Hide();
            }
        }
    }
}
