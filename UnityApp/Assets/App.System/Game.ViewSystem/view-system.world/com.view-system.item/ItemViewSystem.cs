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
        IUpdate<Item>
    {
        public void Awake(Item entity)
        {
        }

        public void Init(Item entity)
        {
            var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Bullet"));
            ModelViewSystem.SetModel(entity, modelObj);
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
