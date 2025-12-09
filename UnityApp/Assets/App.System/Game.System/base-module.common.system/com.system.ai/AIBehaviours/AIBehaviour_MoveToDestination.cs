using ECS;
using ECSGame;

namespace ECSGame
{
    /// <summary>
    /// 移动到目标点
    /// </summary>
    public class AIBehaviour_MoveToDestination : IAIBehaviour
    {
        public AINode StartBehaviour(EcsEntity entity, long behaviourId)
        {
            return AISystem.CreateNode<MoveToAIAction>(behaviourId, entity).StartNode();
        }

        public void MoveNext(AINode aiNode)
        {
            AISystem.StopBehaviour(aiNode.Entity, aiNode.BehaviourId);
            // AISystem.StartBehaviour<AIBehaviour_Caution>(aiNode.Entity);
        }
    }
}