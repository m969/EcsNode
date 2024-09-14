using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : EcsEntity
{
    public int ActorType { get; set; }
}

public class MoveComponent : IEcsComponent
{
    public long Id { get; set; }
    public Vector3 Position { get; set; }
}

public class HealthComponent : IEcsComponent
{
    public long Id { get; set; }
    public int Health { get; set; }
}