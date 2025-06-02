using ECS;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IUpdate<Actor>,
        IAfterRunEvent
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {
            entity.AddComponent<ModelViewComponent>();
            var viewObj = GameObject.Instantiate(Resources.Load<GameObject>("Actor"));
            ModelViewSystem.SetModelTrans(entity, viewObj.transform);
        }

        public void AfterRunEvent(EcsEntity entity, IEventRun eventRun)
        {
            if (eventRun is FireEvent fireEvent)
            {
                SoundSystem.PlayClip(SoundType.OnceFire);
            }
        }

        public void Update(Actor entity)
        {
            EntityViewSystem.Update(entity);
        }
    }
}
