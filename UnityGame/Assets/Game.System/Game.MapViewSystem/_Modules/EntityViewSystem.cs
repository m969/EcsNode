using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class EntityViewSystem : AEntitySystem<EcsEntity>,
IAwake<EcsEntity>,
IInit<EcsEntity>
    {
        public void Awake(EcsEntity entity)
        {
        }

        public void Init(EcsEntity entity)
        {

        }

        public static void Update(EcsEntity entity)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                ModelViewSystem.Update(entity, modelComp);
            }
            if (entity.GetComponent<TransformComponent>() is { } transComp)
            {
                TransformViewSystem.Update(entity, transComp);
            }
        }
    }
}
