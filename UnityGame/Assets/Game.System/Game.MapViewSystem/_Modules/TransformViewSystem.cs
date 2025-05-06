using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class TransformViewSystem : AComponentSystem<EcsEntity, TransformComponent>,
        IAwake<EcsEntity, TransformComponent>,
        IOnChange<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {

        }

        public void OnChange(EcsEntity entity, TransformComponent component)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                var modelTrans = modelComp.ModelTrans;
                modelTrans.GetChild(0).forward = component.Forward.ToVector();

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

        public static void Update(EcsEntity entity, TransformComponent component)
        {
        }
    }
}
