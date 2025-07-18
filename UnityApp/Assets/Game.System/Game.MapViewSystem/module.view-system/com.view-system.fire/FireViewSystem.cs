using ECS;
using ECS.Fody;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TrueSync;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class FireViewSystem : AComponentSystem<EcsEntity, FireComponent>,
        IAwake<EcsEntity, FireComponent>
    {
        public void Awake(EcsEntity entity, FireComponent component)
        {

        }

        [After(typeof(FireSystem), nameof(FireSystem.FireOnce))]
        public static void OnFireOnce(Actor actor, TSVector target)
        {
            SoundSystem.PlayClip(SoundType.OnceFire);
        }
    }
}
