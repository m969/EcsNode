using ECS;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class HudUIComponent : EcsComponent
    {
        public GObject HudObject { get; set; }
    }
}