using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public struct MoveCmd : ICommand
    {
        public Actor Actor;
        public TSVector Pos;
        public TSVector AfterPos;
    }

    public struct EntityViewUpdateCmd : ICommand
    {
        public EcsEntity Entity;
        public EcsComponent Component;
        public object Args;
    }
}
