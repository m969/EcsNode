using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class EcsNodeUnitySystem : AEntitySystem<EcsNode>, IUpdate<EcsNode>
    {
        public void Update(EcsNode ecsNode)
        {
            if (ecsNode.GetComponent<ECSUnity.UIComponent>() is { } uiComponent)
            {
                ECSUnity.UISystem.Update(ecsNode, uiComponent);
            }
        }
    } 
}
