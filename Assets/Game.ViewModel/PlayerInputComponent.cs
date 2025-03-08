using ECS;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputComponent : EcsComponent
{
    public readonly List<PlayerInput> PlayerInputs = new();
}