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
            EventSystem.Update(ecsNode);
        }

        public static T Create<T>(ushort nodeIndex, Assembly systemAssembly) where T : EcsNode
        {
            var ecsNode = (T)Activator.CreateInstance(typeof(T), new object[] { nodeIndex });
            ecsNode.Id = ecsNode.NewEntityId();
            ecsNode.InstanceId = ecsNode.NewInstanceId();
            ecsNode.RegisterDrives(typeof(EcsNode).Assembly.GetTypes());

            var allTypes = systemAssembly.GetTypes();
            ecsNode.RegisterSystems(allTypes);

            //ecsNode.AddComponent<EventComponent>();
            ecsNode.AddComponent<TimerComponent>();
            ecsNode.AddComponent<ReloadComponent>();
            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;

            return ecsNode;
        }
    }
}
