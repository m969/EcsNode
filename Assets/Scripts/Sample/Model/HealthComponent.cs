using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : EcsComponent
{
    public int Health { get; set; }
}