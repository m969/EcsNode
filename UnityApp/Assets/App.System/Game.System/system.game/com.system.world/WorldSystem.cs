using cfg.data;
using ECS;
using ECSGame.Module.GridBased;
using ECSGame.Module.Building;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TrueSync;

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

            // 创建网格区域
            var config = GridPlaneConfig.GetOrDefault(1001);
            var gridPlane = GridPlaneSystem.CreateGridPlane(world, GridPlaneConfigWrap.Create(config), (0, 0));
            gridPlane.AddComponent<TransformComponent>();
            TransformSystem.ChangeRotation(gridPlane, TSQuaternion.Euler(90, 0, 0));
            GridPlaneListSystem.AddGridPlane(world, gridPlane);
            gridPlane.Init();

            // 创建建筑
            var building = BuildingSystem.Create(world, 1, new Vector2Int() { x = 1, y = 1 }, 1);
            building.AddComponent<TransformComponent>();
        }
    }
}
