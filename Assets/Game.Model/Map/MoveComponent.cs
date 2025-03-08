using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

public class MoveComponent : EcsComponent
{
    public int Speed { get; set; }
    public TSVector TrueDirection { get; set; }
}
