using ECS;
using ECSGame.Module.GridBased;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class WorldSystem : AEntitySystem<World>,
    IInit<World>
    {
        public static World Create(Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<World>(EcsType.World, systemAssembly);
            return game;
        }

        public void Init(World world)
        {
            world.AddComponent<GridPlaneListComponent>();
            var gridPlane = GridPlaneSystem.CreateGridPlane(world, 1, (0, 0));
            //GridPlaneListSystem.RegisterGridPlane(gridPlane);
        }
    }
}
