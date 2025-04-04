using ECS;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace ECSUnity
{
    [Serializable]
    public class SoundData
    {
        [ToggleGroup("Enable", "$Type")]
        public bool Enable = true;
        [ToggleGroup("Enable", "$Type")]
        public string Type;
        [ToggleGroup("Enable", "$Type")]
        public AudioClip AudioClip;
        [ToggleGroup("Enable", "$Type")]
        public float Duration;
        [ToggleGroup("Enable", "$Type")]
        [Range(0f, 1f)]
        public float Volume;
        [ToggleGroup("Enable", "$Type")]
        [Range(-3f, 3f)]
        public float Pitch;
    }

    [CreateAssetMenu(fileName = "MixerConfig", menuName = "MixerConfig")]
    public class MixerConfigObject : ScriptableObject
    {
        public AudioMixerGroup MixerGroup;

        public List<SoundData> SoundDatas = new List<SoundData>();
    }
}