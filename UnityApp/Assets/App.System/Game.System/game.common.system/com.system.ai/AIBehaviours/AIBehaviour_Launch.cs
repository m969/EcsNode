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
            //var component = aiNode.AIComponent;
            //var queue = component.Behaviour2NodeQueue[aiNode.BehaviourId];

            var action = aiNode.AIAction;
            if (action is IdleAIAction) aiNode.StartAction<ChaseAIAction>();
            if (action is ChaseAIAction) aiNode.StartAction<CombatIdleAIAction>();
            if (action is CombatIdleAIAction) aiNode.StartAction<AttackAIAction>();
            if (action is AttackAIAction) aiNode.StartAction<CombatIdleAIAction>();
        }
    }
}