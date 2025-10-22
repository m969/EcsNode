using ECS;
using ECSGame;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class SoundSystem : AEntitySystem<SoundMaster>,
        IInit<SoundMaster>
    {
        public void Init(SoundMaster entity)
        {
            entity.MixerConfigs.Add(Resources.Load<MixerConfigObject>("Audios/MixerConfig"));
        }

        public static SoundMaster Create(Assembly systemAssembly)
        {
            var ecsNode = EcsNodeSystem.Create<SoundMaster>(EcsType.Sound, systemAssembly);
            return ecsNode;
        }

        public static void PlayClip(int clipType)
        {
            var soundMaster = EcsDomain.SoundMaster;
            //if (!component.Id2Clip.TryGetValue(clipType, out var audioClip))
            //{
            //    audioClip = Resources.Load<AudioClip>("Smith & Wesson M&P 40C Shot 3");
            //    component.Id2Clip[clipType] = audioClip;
            //}

            soundMaster.Id2Source.TryGetValue(clipType, out var audioSource);
            if (UnityAppStatic.SoundEditorTest)
            {
                if (audioSource != null)
                {
                    GameObject.Destroy(audioSource.gameObject);
                    audioSource = null;
                }
            }
            if (audioSource == null)
            {
                var prefabName = string.Empty;
                if (clipType == SoundType.OnceFire) prefabName = "Sound_OnceFire";
                if (clipType == SoundType.Explosion) prefabName = "Sound_Explosion";

                var prefab = Resources.Load<GameObject>(prefabName);
                var obj = GameObject.Instantiate(prefab);
                audioSource = obj.GetComponent<AudioSource>();
                soundMaster.Id2Source[clipType] = audioSource;
            }

            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
