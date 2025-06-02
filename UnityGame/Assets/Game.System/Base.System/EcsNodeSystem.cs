using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class EcsNodeSystem : AEntitySystem<EcsNode>, IUpdate<EcsNode>
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

        public static EcsNode Create(ushort nodeIndex, Assembly systemAssembly)
        {
            var ecsNode = new EcsNode(nodeIndex);
            ecsNode.Id = ecsNode.NewEntityId();
            ecsNode.InstanceId = ecsNode.NewInstanceId();
            ecsNode.RegisterDrives(typeof(EcsNode).Assembly.GetTypes());

            var allTypes = systemAssembly.GetTypes();
            ecsNode.RegisterSystems(allTypes);

            ecsNode.AddComponent<EventComponent>();
            ecsNode.AddComponent<TimerComponent>();
            ecsNode.AddComponent<ReloadComponent>();

            return ecsNode;
        }
    }
}
