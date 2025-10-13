using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using System;

namespace ECSGame
{
    public class AttackAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            ConsoleLog.Debug("AttackAIAction Awake");
            AttackModule.AttackActionSystem.TryStartAttack(aiNode.Entity, 0, out var failureReason, out var newActionId);
            aiNode.AIComponent.AttackTime = Time.time + 2f;
        }

        public void Update(AINode aiNode)
        {
            if (Time.time >= aiNode.AIComponent.AttackTime)
            {
                AISystem.MoveNext(aiNode);
            }
        }
    }
}
