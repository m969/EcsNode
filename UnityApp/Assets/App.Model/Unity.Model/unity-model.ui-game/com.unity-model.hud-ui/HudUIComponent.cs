using ECS;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ECSUnity
{
    public class HudUIComponent : EcsComponent
    {
        public GObject HudObject { get; set; }
        public Canvas HudCanvas { get; set; }
        public Slider HealthSlider { get; set; }
        public GameObject DamagePopupPrefab { get; set; }
    }
}