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
            ConsoleLog.Error($"Destroy ModelViewComponent {entity.Id}");
            if (component.ModelObj != null)
            {
                GameObject.Destroy(component.ModelObj.gameObject);
            }
        }

        public static GameObject GetModel(EcsEntity entity)
        {
            var component = entity.GetComponent<ModelViewComponent>();
            return component.ModelObj;
        }

        public static void SetModel(EcsEntity entity, GameObject modelObj)
        {
            var component = entity.GetComponent<ModelViewComponent>();
            component.ModelObj = modelObj;
            modelObj.transform.position = TransformSystem.GetPosition(entity).ToVector();
            modelObj.transform.rotation = TransformSystem.GetRotation(entity).ToQuaternion();
        }

        public static void UpdateModelView(EcsEntity entity)
        {
            var component = entity.GetComponent<ModelViewComponent>();
            var modelObj = component.ModelObj;
            modelObj.transform.position = TransformSystem.GetPosition(entity).ToVector();
            modelObj.transform.rotation = TransformSystem.GetRotation(entity).ToQuaternion();
        }
        
        public static void Update(EcsEntity entity)
        {
            if (entity.TryGetComponent<ModelViewComponent>(out var component) == false)
            {
                return;
            }
            var modelTrans = component.ModelObj;
            if (modelTrans == null)
            {
                return;
            }
        }
    }
}
