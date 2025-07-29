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
    public class UIShowWindowEvent : IDomainEvent
    {
        public Type WindowType { get; set; }
        public Action<IUIWindow> BeforeAwake { get; set; }
    }
}
