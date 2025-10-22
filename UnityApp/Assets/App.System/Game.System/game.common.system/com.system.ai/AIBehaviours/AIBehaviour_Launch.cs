using ECS;
using ECSGame;

namespace ECSGame
{
    /// <summary>
    /// 出击
    /// </summary>
    public class AIBehaviour_Launch : IAIBehaviour
    {
        public AINode StartBehaviour(EcsEntity entity, long behaviourId)
        {
            return AISystem.CreateNode<IdleAIAction>(behaviourId, entity).StartNode();
        }

        public void MoveNext(AINode aiNode)
        {
            var action = aiNode.AIAction;
            if (action is IdleAIAction) aiNode.StartAction<ChaseAIAction>();
        }
    }
}