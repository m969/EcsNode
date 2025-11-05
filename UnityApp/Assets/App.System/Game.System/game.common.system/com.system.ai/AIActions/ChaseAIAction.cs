using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using ECSGame.ChaseModule;

namespace ECSGame
{
    public class ChaseAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            ConsoleLog.Debug($"ChaseAIAction Awake: EntityId={aiNode.Entity.Id} {aiNode.Entity.GetComponent<ChaseComponent>().CurrentTargetId}");
            var target = ChaseSystem.GetCurrentTarget(aiNode.Entity);
            if (target == null)
            {
                ConsoleLog.Error("ChaseAIAction Awake: No target to chase");
                return;
            }
            ChaseSystem.StartChase(aiNode.Entity);
            var direction = TSVector.Normalize(TransformSystem.GetPosition(target) - TransformSystem.GetPosition(aiNode.Entity));
            MoveSystem.ChangeDirection(aiNode.Entity, direction);
            TransformSystem.ChangeForward(aiNode.Entity, direction);
            MoveSystem.StartMove(aiNode.Entity);
        }

        public void Update(AINode aiNode)
        {
            var chaseState = ChaseSystem.GetState(aiNode.Entity);
            if (chaseState == ChaseState.Following)
            {
                if (ChaseSystem.HasActiveTarget(aiNode.Entity))
                {
                    var target = ChaseSystem.GetCurrentTarget(aiNode.Entity);
                    MoveSystem.ChangeDirection(aiNode.Entity, TSVector.Normalize(TransformSystem.GetPosition(target) - TransformSystem.GetPosition(aiNode.Entity)));
                    
                    var distance = Vector3.Distance(TransformSystem.GetPosition(aiNode.Entity).ToVector(), TransformSystem.GetPosition(target).ToVector());
                    if (distance <= 2.0f)
                    {
                        MoveSystem.StopMove(aiNode.Entity);
                        // ChaseSystem.StopChase(aiNode.Entity);
                        aiNode.StartAction<CombatIdleAIAction>();
                    }
                }
            }
        }
    }
}
