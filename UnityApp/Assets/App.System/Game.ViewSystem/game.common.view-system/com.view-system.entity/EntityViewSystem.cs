using ECS;
using ECSGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSUnity
{
    public class EntityViewSystem : AEntitySystem<EcsEntity>,
        IAwake<EcsEntity>,
        IInit<EcsEntity>,
        IAfterInit<EcsEntity>
    {
        public void Awake(EcsEntity entity)
        {
            entity.AddComponent<EntityObjComponent>();
            entity.AddComponent<AnimationComponent>();
        }

        public void Init(EcsEntity entity)
        {

        }

        public void AfterInit(EcsEntity entity)
        {

        }

        public static void Update(EcsEntity entity)
        {
            ModelViewSystem.Update(entity);
            TransformViewSystem.Update(entity);
        }
    }
}
