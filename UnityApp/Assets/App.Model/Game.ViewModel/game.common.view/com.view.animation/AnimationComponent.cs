using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using ECS;

namespace ECSGame
{
    public enum AnimationState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Die
    }

    public class AnimationComponent : EcsComponent
    {
        public Animator Animator { get; set; }
        public AnimationState CurrentState { get; set; }
    }
}
