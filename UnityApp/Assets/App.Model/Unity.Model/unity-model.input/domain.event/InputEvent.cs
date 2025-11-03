using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class InputEvent : IEvent
    {
        public InputType InputType { get; set; }
        public Vector3 Direction { get; set; }
    }

        public class FireEvent : IEvent
    {
        public InputType InputType { get; set; }
        public Vector3 Direction { get; set; }
    }

    public interface IInputEventHandler : IDispatch
    {
        void OnHandleInputEvent(EcsNode ecsNode, InputEvent inputEvent);
    }
}
