using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public struct EntityUpdateCmd : ICommand
    {
        public EcsEntity Entity;
        public EcsComponent ChangeComponent;
        public object Args;
    }

    public struct EntityCreateCmd : ICommand
    {
        public EcsEntity Entity;
    }

    public struct EntityDestroyCmd : ICommand
    {
        public EcsEntity Entity;
    }
}
