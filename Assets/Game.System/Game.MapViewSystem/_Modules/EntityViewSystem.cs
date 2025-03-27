using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;

namespace ECSGame
{
    public class EntityViewSystem : AComponentSystem<EcsEntity, EntityViewComponent>,
IAwake<EcsEntity, EntityViewComponent>,
IDestroy<EcsEntity, EntityViewComponent>
    {
        public void Awake(EcsEntity entity, EntityViewComponent component)
        {
            //ConsoleLog.Debug("EntityViewSystem Awake");
            if (entity is Actor)
            {
                var viewObj = GameObject.Instantiate(Resources.Load<GameObject>("Actor"));
                component.ViewObj = viewObj;
            }
            if (entity is Item)
            {
                var viewObj = GameObject.Instantiate(Resources.Load<GameObject>("Bullet"));
                component.ViewObj = viewObj;
            }
            component.ViewObj.transform.position = entity.GetComponent<TransformComponent>().Position.ToVector();
            component.ViewObj.transform.forward = entity.GetComponent<TransformComponent>().Forward.ToVector();
        }

        public void Destroy(EcsEntity entity, EntityViewComponent component)
        {
            if (component.ViewObj != null)
            {
                GameObject.Destroy(component.ViewObj);

                var prefab = Resources.Load<GameObject>("Explosion");
                var explosion = GameObject.Instantiate(prefab);
                explosion.transform.position = TransformSystem.GetPosition(entity).ToVector();
                GameObject.Destroy(explosion, explosion.GetComponent<ScaleTween>().Duration);
            }
        }

        public static void Update(EcsEntity entity, EntityViewComponent component)
        {
            if (component.ViewObj == null)
            {
                return;
            }

            var viewObj = component.ViewObj;
            var newPos = entity.GetComponent<TransformComponent>().Position.ToVector();
            //viewObj.transform.position = newPos;
            viewObj.transform.position = Vector3.Lerp(viewObj.transform.position, newPos, 0.5f);
        }

        public static void SetScale(EcsEntity entity, TSVector scale)
        {
            var viewComp = entity.GetComponent<EntityViewComponent>();
            viewComp.ViewObj.transform.localScale = scale.ToVector();
        }
    }
}
