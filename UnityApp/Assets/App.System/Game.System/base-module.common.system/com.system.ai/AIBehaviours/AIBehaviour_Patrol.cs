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
            return AISystem.CreateNode<MoveInputAIAction>(behaviourId, entity).StartNode();
        }

        public void MoveNext(AINode aiNode)
        {
            var action = aiNode.AIAction;
            // if (action is MoveInputAIAction) aiNode.StartAction<StopMoveInputAIAction>();
            // if (action is StopMoveInputAIAction) aiNode.StartAction<WaitAIAction>();
            if (action is WaitAIAction) aiNode.StartAction<MoveInputAIAction>();
        }
    }
}