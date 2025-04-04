using ECS;
using ECSUnity;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSUnity
{
    public class SoundSystem : AComponentSystem<EcsNode, SoundComponent>,
IAwake<EcsNode, SoundComponent>,
IInit<EcsNode, SoundComponent>
    {
        public void Awake(EcsNode entity, SoundComponent component)
        {
            component.MixerConfigs.Add(Resources.Load<MixerConfigObject>("MixerConfig"));
        }

        public void Init(EcsNode entity, SoundComponent component)
        {
        }

        public static void PlayClip(int clipType)
        {
            var component = StaticObject.EcsNode.GetComponent<SoundComponent>();
            //if (!component.Id2Clip.TryGetValue(clipType, out var audioClip))
            //{
            //    audioClip = Resources.Load<AudioClip>("Smith & Wesson M&P 40C Shot 3");
            //    component.Id2Clip[clipType] = audioClip;
            //}

            component.Id2Source.TryGetValue(clipType, out var audioSource);
            if (StaticObject.SoundEditorTest)
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
                component.Id2Source[clipType] = audioSource;
            }

            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
