using ECS;
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
        public string Type;
        public AudioClip AudioClip;
        public float Duration;
        public float Volume;
        public float Pitch;
    }

    [CreateAssetMenu(fileName = "MixerConfig", menuName = "MixerConfig")]
    public class MixerConfigObject : ScriptableObject
    {
        public AudioMixerGroup MixerGroup;

        public List<SoundData> SoundDatas = new List<SoundData>();
    }
}