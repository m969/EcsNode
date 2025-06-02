using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class CollisionEvent : AEventRun<EcsEntity, EcsEntity>
    {
        public override EcsNode EcsNode { get; set; }
        public EcsEntity Entity1 { get; private set; }
        public EcsEntity Entity2 { get; private set; }

        protected override async ETTask Run(EcsEntity entity1, EcsEntity entity2)
        {
            Entity1 = entity1;
            Entity2 = entity2;

            if (entity1 is Item)
            {
                EcsObject.Destroy(entity1);
            }
            if (entity2 is Item)
            {
                EcsObject.Destroy(entity2);
            }
        }
    }
}
