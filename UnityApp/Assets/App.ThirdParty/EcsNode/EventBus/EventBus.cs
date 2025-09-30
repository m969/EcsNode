using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public interface IEvent
    {
    }

    public abstract class AAsyncEvent : IEvent
    {
        public List<ET.ETTask> HandlerTasks { get; set; } = new List<ET.ETTask>();
    }

    public abstract class ANetworkAsyncEvent : AAsyncEvent
    {

    }

    public class EventBus
    {
        public static EventBus Instance = new EventBus();

        public List<EcsNode> EcsNodes = new List<EcsNode>();


        public void AddEcsNode(EcsNode ecsNode)
        {
            if (!EcsNodes.Contains(ecsNode))
            {
                EcsNodes.Add(ecsNode);
            }
        }

        public void RemoveEcsNode(EcsNode ecsNode)
        {
            if (EcsNodes.Contains(ecsNode))
            {
                EcsNodes.Remove(ecsNode);
            }
        }

        public void Send(IEvent eventObject)
        {
            foreach (var ecsNode in EcsNodes)
            {
                //ecsNode.Dispatch<>
            }
        }
    }
}