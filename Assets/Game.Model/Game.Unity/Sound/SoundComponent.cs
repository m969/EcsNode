using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class SoundComponent : EcsComponent
    {
        public Dictionary<int, AudioClip> Id2Clip { get; set; } = new();
        public Dictionary<int, AudioSource> Id2Source { get; set; } = new();
    }
}