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
            var viewObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            component.ViewObj = viewObj;
            viewObj.transform.position = entity.GetComponent<TrueTransformComponent>().Position.ToVector();
        }

        public static void SetScale(EcsEntity entity, TSVector scale)
        {
            var viewComp = entity.GetComponent<EntityViewComponent>();
            viewComp.ViewObj.transform.localScale = scale.ToVector();
        }
    }
}
