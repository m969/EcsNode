using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using ECS;

namespace ECSGame
{
    public class AnimationComponent : EcsComponent
    {
        public Animator Animator { get; set; }
    }
}
