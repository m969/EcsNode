using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class UIComponent : EcsComponent
    {
        public Dictionary<Type, FairyGUI.GComponent> Type2Windows { get; set; } = new();
        //public Dictionary<Type, string> Type2URL { get; set; } = new();
    }
}