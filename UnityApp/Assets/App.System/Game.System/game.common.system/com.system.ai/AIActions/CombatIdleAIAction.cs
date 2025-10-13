using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using System;

namespace ECSGame
{
    public class CombatIdleAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            ConsoleLog.Debug("CombatIdleAIAction Awake");
            aiNode.AIComponent.IdleTime = Time.time + 1f;
        }

        public void Update(AINode aiNode)
        {
            if (Time.time >= aiNode.AIComponent.IdleTime)
            {
                AISystem.MoveNext(aiNode);
            }
        }
    }
}
