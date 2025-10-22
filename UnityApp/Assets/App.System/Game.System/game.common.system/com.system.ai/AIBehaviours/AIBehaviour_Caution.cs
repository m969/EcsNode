using ECS;
using ECSGame;

namespace ECSGame
{
    /// <summary>
    /// 警戒
    /// </summary>
    public class AIBehaviour_Caution : IAIBehaviour
    {
        public AINode StartBehaviour(EcsEntity entity, long behaviourId)
        {
            return AISystem.CreateNode<CautionIdleAIAction>(behaviourId, entity).StartNode();
        }

        public void MoveNext(AINode aiNode)
        {

        }
    }
}