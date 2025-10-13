using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TrueSync;

namespace ECSGame
{
    public static class AIActionExtension
    {
        //public static AIActionQueue OnNext<T>(this AIActionQueue actionQueue) where T : IAIAction
        //{
        //    actionQueue.ActionQueue.Enqueue(typeof(T));
        //    return actionQueue;
        //}

        private static AINode NextAction<T>(this AINode aiNode) where T : IAIAction, new()
        {
            var newNode = new AINode()
            {
                Id = aiNode.Id,
                BehaviourId = aiNode.BehaviourId,
                Entity = aiNode.Entity,
                AIComponent = aiNode.Entity.GetComponent<AIComponent>(),
                AIAction = ReloadSystem.CreateInstance(aiNode.Entity.EcsNode, typeof(T).FullName) as IAIAction,
                PreNode = aiNode,
            };
            return newNode;
        }

        public static AINode StartAction<T>(this AINode aiNode) where T : IAIAction, new()
        {
            var component = aiNode.AIComponent;
            var queue = component.Behaviour2NodeQueue[aiNode.BehaviourId];
            var newNode = aiNode.NextAction<T>();
            newNode.StartNode();
            queue.Dequeue();
            return newNode;
        }

        public static AINode StartNode(this AINode aiNode)
        {
            AISystem.StartNode(aiNode);
            return aiNode;
        }

        public static void NodeDepthGC(this AINode aiNode, int depth)
        {
            if (aiNode.PreNode != null)
            {
                aiNode.PreNode.NodeDepthGC(depth + 1);
                if (depth > 5)
                {
                    aiNode.PreNode = null;
                }
            }
        }
    }

    public class AISystem : AComponentSystem<EcsEntity, AIComponent>,
        IAwake<EcsEntity, AIComponent>,
        IEnable<EcsEntity, AIComponent>,
        IDisable<EcsEntity, AIComponent>
    {
        public void Awake(EcsEntity entity, AIComponent component)
        {

        }

        public void Enable(EcsEntity entity, AIComponent component)
        {

        }

        public void Disable(EcsEntity entity, AIComponent component)
        {

        }

        public static void Update(EcsEntity entity, AIComponent component, long determineFrame)
        {
            component.DetermineFrame = determineFrame;
            foreach (var queue in component.Behaviour2NodeQueue.Values)
            {
                if (queue.Count == 0)
                {
                    continue;
                }
                var node = queue.Peek();
                node.AIAction.Update(node);
                ConsoleLog.Debug($"AISystem Behaviour: {component.AIBehaviours[node.BehaviourId].GetType().Name} Action: {node.AIAction.GetType().Name}");
            }
        }

        public static AINode StartBehaviour<T>(EcsEntity entity) where T : IAIBehaviour, new()
        {
            var component = entity.GetComponent<AIComponent>();
            var aiBehaviour = new T();
            var behaviourId = entity.EcsNode.NewInstanceId();
            component.AIBehaviours.Add(behaviourId, aiBehaviour);
            return aiBehaviour.StartBehaviour(entity, behaviourId);
        }

        public static AINode CreateNode<T>(EcsEntity entity) where T : IAIAction, new()
        {
            return CreateNode<T>(entity.EcsNode.NewInstanceId(), entity);
        }

        public static AINode CreateNode<T>(long behaviourId, EcsEntity entity) where T : IAIAction, new()
        {
            var nodeId = entity.EcsNode.NewInstanceId();
            var newNode = new AINode()
            {
                Id = nodeId,
                BehaviourId = behaviourId,
                Entity = entity,
                AIComponent = entity.GetComponent<AIComponent>(),
                AIAction = ReloadSystem.CreateInstance(entity.EcsNode, typeof(T).FullName) as IAIAction,
                PreNode = null,
            };
            return newNode;
        }

        public static void StartNode(AINode aiNode)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            if (!component.Behaviour2NodeQueue.ContainsKey(aiNode.BehaviourId))
            {
                var queue = new Queue<AINode>();
                queue.Enqueue(aiNode);
                component.Behaviour2NodeQueue.Add(aiNode.BehaviourId, queue);
            }
            else
            {
                component.Behaviour2NodeQueue[aiNode.BehaviourId].Enqueue(aiNode);
            }

            aiNode.NodeDepthGC(1);

            aiNode.AIAction.Awake(aiNode);
        }

        public static void MoveNext(AINode aiNode)
        {
            aiNode.Entity.GetComponent<AIComponent>().AIBehaviours[aiNode.BehaviourId].MoveNext(aiNode);
            //if (aiNode.AIBehaviour == AIBehaviourType.Patrol)
            //{
            //    PatrolAINext(aiNode);
            //}
        }

        //public static void PatrolAINext(AINode aiNode)
        //{
        //    var component = aiNode.Entity.GetComponent<AIComponent>();
        //    var queue = component.NodeMap[aiNode.Id];

        //    if (aiNode.AIAction is MoveInputAIAction)
        //    {
        //        aiNode.NextAction<StopMoveInputAIAction>().StartNode();
        //    }
        //    if (aiNode.AIAction is StopMoveInputAIAction)
        //    {
        //        aiNode.NextAction<WaitAIAction>().StartNode();
        //    }
        //    if (aiNode.AIAction is WaitAIAction)
        //    {
        //        aiNode.NextAction<MoveInputAIAction>().StartNode();
        //    }

        //    queue.Dequeue();
        //}

        //public static AIActionQueue CreateActionQueue(EcsEntity entity)
        //{
        //    var component = entity.GetComponent<AIComponent>();
        //    var actionQueue = new AIActionQueue() { ActionQueueId = component.ActionQueueIndex++ };
        //    component.AIActionQueues.Add(actionQueue.ActionQueueId, actionQueue);
        //    return actionQueue;
        //}

        //public static async ETTask RunAction(EcsEntity entity, AIActionQueue actionQueue)
        //{
        //    var component = entity.GetComponent<AIComponent>();

        //    while (actionQueue.ActionQueue.Count > 0)
        //    {
        //        var actionType = actionQueue.ActionQueue.Dequeue();
        //        var aiAction = ReloadSystem.CreateInstance(entity.EcsNode, actionType.FullName) as IAIAction;
        //        await aiAction.Run(entity);
        //    }

        //    component.AIActionQueues.Remove(actionQueue.ActionQueueId);
        //}

        //public static async ETTask RepeatAction(EcsEntity entity, AIActionQueue actionQueue)
        //{
        //    var component = entity.GetComponent<AIComponent>();

        //    while (!actionQueue.EndRepeat)
        //    {
        //        var actionType = actionQueue.ActionQueue.Dequeue();
        //        actionQueue.ActionQueue.Enqueue(actionType);
        //        var aiAction = ReloadSystem.CreateInstance(entity.EcsNode, actionType.FullName) as IAIAction;
        //        await aiAction.Run(entity);
        //    }

        //    component.AIActionQueues.Remove(actionQueue.ActionQueueId);
        //}
    }
}
