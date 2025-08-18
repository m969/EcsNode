using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;

namespace ECSGame
{
    public class EntityObjSystem : AComponentSystem<EcsEntity, EntityObjComponent>,
        IAwake<EcsEntity, EntityObjComponent>,
        IDestroy<EcsEntity, EntityObjComponent>,
        IAfterInit<EcsEntity, EntityObjComponent>
    {
        public void Awake(EcsEntity entity, EntityObjComponent component)
        {

        }

        public void AfterInit(EcsEntity entity, EntityObjComponent component)
        {
            Create(entity);
        }

        public void Destroy(EcsEntity entity, EntityObjComponent component)
        {
            if (component.EntityObj != null)
            {
                GameObject.Destroy(component.EntityObj);
            }
        }

        public static GameObject Create(EcsEntity entity)
        {
            var entityObj = new GameObject($"{entity.GetType().Name}({entity.Id})");
            if (entity.Parent != null)
            {
                entityObj.transform.parent = entity.Parent.GetComponent<EntityObjComponent>()?.EntityObj?.transform;
            }
            entity.GetComponent<EntityObjComponent>().EntityObj = entityObj;
            return entityObj;
        }
    }
}
