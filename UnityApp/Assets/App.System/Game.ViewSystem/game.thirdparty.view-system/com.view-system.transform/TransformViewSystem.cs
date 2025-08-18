using ECS;
using ECS.Fody;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TrueSync;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class TransformViewSystem : AComponentSystem<EcsEntity, TransformComponent>,
        IAwake<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {

        }

        [After(typeof(TransformSystem), nameof(TransformSystem.ChangePosition))]
        public static void OnChangePosition(EcsEntity entity, TSVector target)
        {
            //ConsoleLog.Debug("OnChangePosition");
        }

        [After(typeof(TransformSystem), nameof(TransformSystem.ChangeForward))]
        public static void OnChangeForward(EcsEntity entity, TSVector target)
        {
            //ConsoleLog.Debug("OnChangeForward");
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                var modelObj = modelComp.ModelObj;
                modelObj.transform.forward = target.ToVector();
            }
        }

        public static void Update(EcsEntity entity)
        {
            if (entity.TryGetComponent<TransformComponent>(out var component) == false)
            {
                return;
            }
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                var modelObj = modelComp.ModelObj;

                if (entity is Actor)
                {
                    var newPos = component.ForecastPosition.ToVector();
                    modelObj.transform.position = Vector3.Lerp(modelObj.transform.position, newPos, 0.5f);

                    var newPos2 = component.Position.ToVector();
                    modelObj.transform.GetChild(1).position = Vector3.Lerp(modelObj.transform.GetChild(1).position, newPos2, 0.5f);
                }
                else
                {
                    var newPos = component.Position.ToVector();
                    modelObj.transform.position = Vector3.Lerp(modelObj.transform.position, newPos, 0.5f);
                }
            }
        }
    }
}
