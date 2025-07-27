using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;

namespace ECSGame
{
    public class ModelViewSystem : AComponentSystem<EcsEntity, ModelViewComponent>,
IAwake<EcsEntity, ModelViewComponent>,
IDestroy<EcsEntity, ModelViewComponent>
    {
        public void Awake(EcsEntity entity, ModelViewComponent component)
        {

        }

        public void Destroy(EcsEntity entity, ModelViewComponent component)
        {
            if (component.ModelTrans != null)
            {
                GameObject.Destroy(component.ModelTrans.gameObject);

                var prefab = Resources.Load<GameObject>("Explosion");
                var explosion = GameObject.Instantiate(prefab);
                explosion.transform.position = TransformSystem.GetPosition(entity).ToVector();
                GameObject.Destroy(explosion, explosion.GetComponent<ScaleTween>().Duration);
            }
        }

        public static void SetModelTrans(EcsEntity entity, Transform modelTrans)
        {
            var component = entity.GetComponent<ModelViewComponent>();
            component.ModelTrans = modelTrans;
            component.ModelTrans.position = entity.GetComponent<TransformComponent>().Position.ToVector();
            component.ModelTrans.forward = entity.GetComponent<TransformComponent>().Forward.ToVector();
        }

        public static void Update(EcsEntity entity, ModelViewComponent component)
        {
            var modelTrans = component.ModelTrans;
            if (modelTrans == null)
            {
                return;
            }
        }
    }
}
