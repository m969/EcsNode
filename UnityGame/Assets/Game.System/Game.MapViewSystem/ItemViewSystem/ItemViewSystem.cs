using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ItemViewSystem : AEntitySystem<Item>,
IAwake<Item>,
IInit<Item>,
IUpdate<Item>
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

        public void Update(Item entity)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } component)
            {
                ModelViewSystem.Update(entity, component);
            }
        }
    }
}
