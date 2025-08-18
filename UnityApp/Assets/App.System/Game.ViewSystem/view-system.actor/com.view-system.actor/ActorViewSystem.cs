using ECS;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IUpdate<Actor>
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {
            var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Actor"));
            ModelViewSystem.SetModel(entity, modelObj);
        }

        public void Update(Actor entity)
        {
            EntityViewSystem.Update(entity);
        }
    }
}
