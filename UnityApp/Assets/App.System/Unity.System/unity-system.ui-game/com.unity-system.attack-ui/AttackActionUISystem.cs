using ECS;
using ECSGame;
using ECS.Fody;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ECSGame.AttackModule;

namespace ECSUnity
{
    public class AttackActionUISystem : AEntitySystem<AttackAction>, IOnAttackHit
    {
        public void OnAttackHit(EcsEntity owner, AttackAction action, long attackerId, long targetId, int finalDamage)
        {
            ConsoleLog.Error($"AttackActionUISystem OnAttackHit {attackerId} -> {targetId} Damage: {finalDamage}");
            var targetEntity = owner.Parent.GetChild<Actor>(targetId);
            if (targetEntity == null)
            {
                return;
            }
            HudUISystem.NewDamagePopup(targetEntity, finalDamage);
        }
    }
}
