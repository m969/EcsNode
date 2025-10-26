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
            if (queue.Count == 0 || queue.Peek() != aiNode)
            {
                ConsoleLog.Error("AIAction StartAction Error: not current node");
                return null;
            }
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
                // ConsoleLog.Debug($"AISystem Behaviour: {component.AIBehaviours[node.BehaviourId].GetType().Name} Action: {node.AIAction.GetType().Name}");
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
            if (aiNode.Entity.GetComponent<AIComponent>() is {} component)
            {
                component.AIBehaviours[aiNode.BehaviourId].MoveNext(aiNode);
            }
            //if (aiNode.AIBehaviour == AIBehaviourType.Patrol)
            //{
            //    PatrolAINext(aiNode);
            //}
        }

        public static float ReadFloat(AINode aiNode, FloatValue key, out float value)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            if (!component.Int2FloatValues.TryGetValue((int)key, out value))
            {
                value = 0;
            }
            return value;
        }

        public static void WriteFloat(AINode aiNode, FloatValue key, float value)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            component.Int2FloatValues[(int)key] = value;
        }
    }
}
