using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;
using System;

namespace ECSGame
{
    public class CautionIdleAIAction : IAIAction
    {
        public void Awake(AINode aiNode)
        {
            // ConsoleLog.Debug("CautionIdleAIAction Awake");
            //aiNode.AIComponent.IdleTime = Time.time + 2f;
        }

        public void Update(AINode aiNode)
        {
            //if (Time.time >= aiNode.AIComponent.IdleTime)
            //{
            //    AISystem.MoveNext(aiNode);
            //}
            if (AppStatic.MyActor == null) return;
            var distance = Vector3.Distance(TransformSystem.GetPosition(aiNode.Entity).ToVector(), TransformSystem.GetPosition(AppStatic.MyActor).ToVector());
            if (distance <= 2.0f)
            {
                aiNode.StartAction<CombatIdleAIAction>();
            }
        }
    }
}
