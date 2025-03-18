using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;

namespace ECSGame
{
    public class EntityViewSystem : AComponentSystem<EcsEntity, EntityViewComponent>,
IAwake<EcsEntity, EntityViewComponent>
    {
        public void Awake(EcsEntity entity, EntityViewComponent component)
        {
            //ConsoleLog.Debug("EntityViewSystem Awake");
            if (entity is Actor)
            {
                var viewObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
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

        public static void SetScale(EcsEntity entity, TSVector scale)
        {
            var viewComp = entity.GetComponent<EntityViewComponent>();
            viewComp.ViewObj.transform.localScale = scale.ToVector();
        }
    }
}
