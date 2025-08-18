using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using ECSGame;

namespace ECSUnity
{
    public class PlayerInput : EcsNode
    {
        public PlayerInput(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}