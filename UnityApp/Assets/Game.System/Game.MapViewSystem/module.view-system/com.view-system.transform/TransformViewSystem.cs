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
                var modelTrans = modelComp.ModelTrans;
                modelTrans.forward = target.ToVector();
            }
        }

        public static void Update(EcsEntity entity, TransformComponent component)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                var modelTrans = modelComp.ModelTrans;

                if (entity is Actor)
                {
                    var newPos = component.ForecastPosition.ToVector();
                    modelTrans.position = Vector3.Lerp(modelTrans.position, newPos, 0.5f);

                    var newPos2 = component.Position.ToVector();
                    modelTrans.GetChild(1).position = Vector3.Lerp(modelTrans.GetChild(1).position, newPos2, 0.5f);
                }
                else
                {
                    var newPos = component.Position.ToVector();
                    modelTrans.position = Vector3.Lerp(modelTrans.position, newPos, 0.5f);
                }
            }
        }
    }
}
