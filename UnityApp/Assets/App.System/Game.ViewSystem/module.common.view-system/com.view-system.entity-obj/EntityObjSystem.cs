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
            var entityObj = new GameObject($"{entity.GetType().Name} ({entity.GetType().BaseType.Name})");
            GameObject.DontDestroyOnLoad(entityObj);
            if (entity.Parent != null)
            {
                entityObj.transform.parent = entity.Parent.GetComponent<EntityObjComponent>()?.EntityObj?.transform;
            }
            entity.GetComponent<EntityObjComponent>().EntityObj = entityObj;

            var children = entity.Id2Children.Values;

            //一般情况下子实体会先Init完成并生成EntityObj，所以在父实体生成Obj时需要遍历子实体并将其设置到父实体下
            foreach (var child in children)
            {
                var childObj = child.GetComponent<EntityObjComponent>().EntityObj;
                if (childObj != null)
                {
                    childObj.transform.parent = entityObj.transform;
                }
            }

            return entityObj;
        }
    }
}
