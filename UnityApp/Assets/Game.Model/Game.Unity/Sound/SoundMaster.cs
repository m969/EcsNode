using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    /// <summary>
    /// 声音主控器
    /// </summary>
    public class SoundMaster : EcsNode
    {
        public Dictionary<int, AudioClip> Id2Clip { get; set; } = new();
        public Dictionary<int, AudioSource> Id2Source { get; set; } = new();
        public List<MixerConfigObject> MixerConfigs { get; set; } = new();

        public SoundMaster(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}