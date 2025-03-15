using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class PlayerInputComponent : EcsComponent
    {
        public readonly List<PlayerInput> PlayerInputs = new();
        public Vector3 MoveVector { get; set; }
        public Vector3 LookVector { get; set; }
        public VariableJoystick MoveJoystick { get; set; }
        public VariableJoystick LookJoystick { get; set; }
        public VariableJoystick FireJoystick { get; set; }
    } 
}