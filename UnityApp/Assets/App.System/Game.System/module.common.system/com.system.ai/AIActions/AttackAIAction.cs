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
            aiNode.AIComponent.AttackTime = Time.time + 1.2f;
        }

        public void Update(AINode aiNode)
        {
            if (Time.time >= aiNode.AIComponent.AttackTime)
            {
                var targetActor = ChaseSystem.GetCurrentTarget(aiNode.Entity) as Actor;
                if (targetActor != null)
                {
                    HealthSystem.ChangeHealth(targetActor, -30);
                }
                aiNode.StartAction<CombatIdleAIAction>();
            }
        }
    }
}
