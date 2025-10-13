using ECS;
using ECSGame;

namespace ECSGame
{
    /// <summary>
    /// 巡逻
    /// </summary>
    public class AIBehaviour_Patrol : IAIBehaviour
    {
        public AINode StartBehaviour(EcsEntity entity, long behaviourId)
        {
            return AISystem.CreateNode<MoveInputAIAction>(behaviourId, entity).StartNode(); ;
        }

        public void MoveNext(AINode aiNode)
        {
            //var component = aiNode.Entity.GetComponent<AIComponent>();
            //var queue = component.Behaviour2NodeQueue[aiNode.BehaviourId];
            //if (queue.Count > 0)
            //{
            //    queue.Dequeue();
            //}

            if (aiNode.AIAction is MoveInputAIAction) aiNode.StartAction<StopMoveInputAIAction>();
            if (aiNode.AIAction is StopMoveInputAIAction) aiNode.StartAction<WaitAIAction>();
            if (aiNode.AIAction is WaitAIAction) aiNode.StartAction<MoveInputAIAction>();
        }
    }
}