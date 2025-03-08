using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EntitySystem : AEcsEntitySystem<EcsEntity>,
        IAwake<EcsEntity>,
        IInit<EcsEntity>
    {
        /// <summary>
        /// 所有实体的唤醒回调
        /// </summary>
        public void Awake(EcsEntity entity)
        {
            //Debug.Log($"EntitySystem Awake {entity.GetType().Name}");
        }

        /// <summary>
        /// 所有实体的初始化回调
        /// </summary>
        public void Init(EcsEntity entity)
        {
            //Debug.Log($"EntitySystem Init {entity.GetType().Name}");
        }
    }
}