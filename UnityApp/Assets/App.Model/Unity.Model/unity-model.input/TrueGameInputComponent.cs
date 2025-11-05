using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using ECSGame;

namespace ECSUnity
{
    public class TrueGameInputComponent : EcsComponent
    {
        public readonly List<InputData> InputDatas = new();

        // public TrueWorld TrueWorld { get; set; }
        // public Actor PlayerActor { get; set; }
        public Vector3 MoveVector { get; set; }
        public Vector3 LookVector { get; set; }
        public Vector3 FireVector { get; set; }
        public bool FireState { get; set; }
        public FP NextFireTime { get; set; }
        public VariableJoystick MoveJoystick { get; set; }
        public VariableJoystick LookJoystick { get; set; }
        public VariableJoystick FireJoystick { get; set; }
    }
}