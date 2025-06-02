using ECS;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ItemViewSystem : AEntitySystem<Item>,
        IAwake<Item>,
        IInit<Item>,
        IDestroy<Item>,
        IUpdate<Item>,
        IAfterRunEvent
    {
        public void Awake(Item entity)
        {
        }

        public void Init(Item entity)
        {
            entity.AddComponent<ModelViewComponent>();
            var viewObj = GameObject.Instantiate(Resources.Load<GameObject>("Bullet"));
            ModelViewSystem.SetModelTrans(entity, viewObj.transform);
        }

        public void AfterRunEvent(EcsEntity entity, IEventRun eventRun)
        {
            if (eventRun is CollisionEvent collisionEvent)
            {

            }
        }

        public void Update(Item entity)
        {
            EntityViewSystem.Update(entity);
        }

        public void Destroy(Item entity)
        {
            SoundSystem.PlayClip(SoundType.Explosion);
        }
    }
}
