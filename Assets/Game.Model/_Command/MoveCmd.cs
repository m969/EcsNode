using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

public struct MoveCmd : ICommand
{
    public Actor Actor;
    public TSVector Pos;
    public TSVector AfterPos;
}
