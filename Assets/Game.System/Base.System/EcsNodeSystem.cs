using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class EcsNodeSystem : IUpdate<EcsNode>
    {
        public void Update(EcsNode ecsNode)
        {
            if (ecsNode.GetComponent<TimerComponent>() is { } timerComponent)
            {
                TimerSystem.Update(ecsNode, timerComponent);
            }
            if (ecsNode.GetComponent<EventComponent>() is { } eventComponent)
            {
                EventSystem.Update(ecsNode, eventComponent);
            }
        }
    } 
}
