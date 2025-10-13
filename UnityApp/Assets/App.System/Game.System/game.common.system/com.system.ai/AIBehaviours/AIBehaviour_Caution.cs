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
            return AISystem.CreateNode<CautionIdleAIAction>(behaviourId, entity).StartNode(); ;
        }

        //public AINode StartAction<T>(AINode aiNode) where T : IAIAction, new()
        //{
        //    var component = aiNode.AIComponent;
        //    var queue = component.Behaviour2NodeQueue[aiNode.BehaviourId];
        //    var newNode = aiNode.StartAction<T>();
        //    queue.Dequeue();
        //    return newNode;
        //}

        public void MoveNext(AINode aiNode)
        {
            //var component = aiNode.AIComponent;
            //var queue = component.Behaviour2NodeQueue[aiNode.BehaviourId];

            //if (aiNode.AIAction is CautionIdleAIAction)
            //{
            //    aiNode.StartAction<ChaseAIAction>();
            //}
            //if (aiNode.AIAction is ChaseAIAction)
            //{
            //    aiNode.StartAction<CombatIdleAIAction>();
            //}
            //if (aiNode.AIAction is CombatIdleAIAction)
            //{
            //    aiNode.StartAction<AttackAIAction>();
            //}
        }
    }
}