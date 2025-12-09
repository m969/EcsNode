using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;

namespace ECSGame
{
    public class ModelViewComponent : EcsComponent
    {
        public GameObject ModelObj { get; set; }
    }
}
