using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using System;
using ECSGame.ChaseModule;

namespace ECSGame
{
    public class AttackAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            ConsoleLog.Debug("AttackAIAction Awake");
            // var targetActor = ChaseSystem.GetCurrentTarget(aiNode.Entity);
            // if (targetActor == null)
            // {
            //     ConsoleLog.Debug("AttackAIAction: No target to attack.");
            //     aiNode.StartAction<CombatIdleAIAction>();
            //     return;
            // }
            // AttackModule.AttackActionSystem.TryStartAttack(aiNode.Entity, targetActor.Id, out var failureReason, out var newActionId);
            aiNode.AIComponent.AttackTime = Time.time + 1.2f;
        }

        public void Update(AINode aiNode)
        {
            if (Time.time >= aiNode.AIComponent.AttackTime)
            {
                var targetActor = ChaseSystem.GetCurrentTarget(aiNode.Entity) as Actor;
                HealthSystem.ChangeHealth(targetActor, -10);
                aiNode.StartAction<CombatIdleAIAction>();
            }
        }
    }
}
