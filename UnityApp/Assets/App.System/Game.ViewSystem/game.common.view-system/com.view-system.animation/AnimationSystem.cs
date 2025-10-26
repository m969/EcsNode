using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;
using ECS.Fody;

namespace ECSGame
{
    public class AnimationSystem : AComponentSystem<EcsEntity, AnimationComponent>,
        IAwake<EcsEntity, AnimationComponent>,
        IDestroy<EcsEntity, AnimationComponent>
    {
        public void Awake(EcsEntity entity, AnimationComponent component)
        {

        }

        public void Destroy(EcsEntity entity, AnimationComponent component)
        {

        }

        [After(typeof(ModelViewSystem), nameof(ModelViewSystem.SetModel))]
        public static void OnSetModel(EcsEntity entity, GameObject modelObj)
        {
            var component = entity.GetComponent<AnimationComponent>();
            component.Animator = modelObj.GetComponentInChildren<Animator>();
        }

        public static void Play(EcsEntity entity, AnimationState animState)
        {
            var component = entity.GetComponent<AnimationComponent>();
            if (component.Animator != null)
            {
                if (component.CurrentState == animState)
                {
                    return;
                }
                component.CurrentState = animState;
                var animName = animState.ToString();
                if (animState == AnimationState.Attack)
                {
                    animName = "Attack01";
                }
                component.Animator.CrossFade(animName, 0.15f);
            }
        }

        [After(typeof(AISystem), nameof(AISystem.StartNode))]
        public static void OnStartNode(AINode aiNode)
        {
            if (aiNode.AIAction is IdleAIAction)
                Play(aiNode.Entity, AnimationState.Idle);
            else if (aiNode.AIAction is ChaseAIAction)
                Play(aiNode.Entity, AnimationState.Walk);
            else if (aiNode.AIAction is AttackAIAction)
                Play(aiNode.Entity, AnimationState.Attack);
            else if (aiNode.AIAction is CombatIdleAIAction)
                Play(aiNode.Entity, AnimationState.Idle);
        }
    }
}
