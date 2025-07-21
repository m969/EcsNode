using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class UIStage : EcsNode
    {
        public Dictionary<Type, FairyGUI.GComponent> Type2Windows { get; set; } = new();

        public UIStage(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}