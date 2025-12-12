using ECS;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class TrueWorldViewSystem : AEntitySystem<TrueWorld>,
        IEventHandle<CollisionEvent>
    {
        public void OnHandleEvent(EcsNode ecsNode, CollisionEvent eventContext)
        {
            if (eventContext.Self is Item item)
            {
                var component = item.GetComponent<ModelViewComponent>();
                if (component.ModelObj != null)
                {
                    var prefab = Resources.Load<GameObject>("Explosion");
                    var explosion = GameObject.Instantiate(prefab);
                    explosion.transform.position = TransformSystem.GetPosition(item).ToVector();
                    GameObject.Destroy(explosion, explosion.GetComponent<ScaleTween>().Duration);
                }
            }
        }
    }
}
