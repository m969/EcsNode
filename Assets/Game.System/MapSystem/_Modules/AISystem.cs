using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TrueSync;
using static UnityEngine.EventSystems.EventTrigger;

namespace ECSGame
{
    public static class AIActionExtension
    {
        public static AIActionQueue OnNext<T>(this AIActionQueue actionQueue) where T : IAIAction
        {
            actionQueue.ActionQueue.Enqueue(typeof(T));
            return actionQueue;
        }

        public static AINode NextAction<T>(this AINode aiNode) where T : IAIAction, new()
        {
            var newNode = new AINode()
            {
                Id = aiNode.Id,
                AIBehaviour = aiNode.AIBehaviour,
                Entity = aiNode.Entity,
                AIAction = ReloadSystem.CreateInstance(aiNode.Entity.EcsNode, typeof(T).FullName) as IAIAction,
                PreNode = aiNode,
            };
            return newNode;
        }
    }

    public class AISystem : AComponentSystem<EcsEntity, AIComponent>,
IAwake<EcsEntity, AIComponent>
    {
        public void Awake(EcsEntity entity, AIComponent component)
        {
            //var actionQueue = CreateActionQueue(entity);
            //actionQueue.OnNext<MoveAIAction>().OnNext<IdleAIAction>();
            //RepeatAction(entity, actionQueue).Coroutine();

            //var actionQueue2 = CreateActionQueue(entity);
            //actionQueue2.OnNext<PatrolAIAction>();
            //RunAction(entity, actionQueue2).Coroutine();

            var aiNode = new AINode()
            {
                Id = 1,
                AIBehaviour = AIBehaviourType.Patrol,
                Entity = entity,
                AIAction = ReloadSystem.CreateInstance(entity.EcsNode, typeof(MoveAIAction).FullName) as IAIAction
            };

            StartNode(aiNode);
        }

        public static void FrameUpdate(EcsEntity entity, AIComponent component)
        {
            //ConsoleLog.Debug($"AISystem FrameUpdate {component.NodeMap.Count}");
            foreach (var queue in component.NodeMap.Values)
            {
                var node = queue.Peek();
                //ConsoleLog.Debug($"{node.AIAction.GetType().Name}");
                node.AIAction.Run(node);
            }
        }

        public static void StartNode(AINode aiNode)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            if (!component.NodeMap.ContainsKey(aiNode.Id))
            {
                var queue = new Queue<AINode>();
                queue.Enqueue(aiNode);
                component.NodeMap.Add(aiNode.Id, queue);
            }
            else
            {
                component.NodeMap[aiNode.Id].Enqueue(aiNode);
            }

            aiNode.AIAction.Start(aiNode);
        }

        public static void FinishProcess(AINode aiNode)
        {
            if (aiNode.AIBehaviour == AIBehaviourType.Patrol)
            {
                PatrolAIProcess(aiNode);
            }
        }

        public static void PatrolAIProcess(AINode aiNode)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            var queue = component.NodeMap[aiNode.Id];
            if (aiNode.AIAction is WaitAIAction)
            {
                if (aiNode.PreNode != null)
                {
                    if (aiNode.PreNode.AIAction is IdleAIAction)
                    {
                        var newNode = aiNode.NextAction<MoveAIAction>();
                        StartNode(newNode);
                    }
                    if (aiNode.PreNode.AIAction is MoveAIAction)
                    {
                        var newNode = aiNode.NextAction<IdleAIAction>();
                        StartNode(newNode);
                    }
                }
            }
            if (aiNode.AIAction is IdleAIAction || aiNode.AIAction is MoveAIAction)
            {
                var newNode = aiNode.NextAction<WaitAIAction>();
                StartNode(newNode);
            }
            queue.Dequeue();
        }

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
