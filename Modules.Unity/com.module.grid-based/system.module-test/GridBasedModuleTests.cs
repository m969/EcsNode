using ECS;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ECSGame.Module.GridBased;

namespace ECSGame.Module.GridBased.Tests
{
    [TestFixture]
    public class GridBasedModuleTests
    {
        private EcsNode ecsNode = null!;
        private MockGridPlaneConfig config = null!;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        // 模拟 IGridPlaneConfig 实现
        public class MockGridPlaneConfig : EcsComponent, IGridPlaneConfig
        {
            public int Id { get; set; }
            public string Key { get; set; } = string.Empty;
            public int Width { get; set; }
            public int Height { get; set; }
            public float CellSize { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
            config = new MockGridPlaneConfig()
            {
                Id = 5,
                Width = 10,
                Height = 15,
                CellSize = 2.5f,
            };
        }

        [Test]
        public void CreateGridPlane_ValidParameters_ShouldCreateGridPlaneEntity()
        {
            int configId = 42;
            var position = (x: 1.5f, y: 2.5f);
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, position);

            Assert.That(plane, Is.Not.Null);
            Assert.That(plane.ConfigId, Is.EqualTo(configId));
            Assert.That(plane.Position, Is.EqualTo(position));
            Assert.That(plane.Parent, Is.EqualTo(ecsNode));
        }

        [Test]
        public void CreateGridPlane_InvalidConfigId_ShouldThrowException()
        {
            // 负值或不存在的配置ID应抛出参数异常
            Assert.That(() => GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CreateGridCell_ValidParameters_ShouldCreateGridCellEntity()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var cell = GridCellSystem.CreateGridCell(plane, 2, 3);

            Assert.That(cell, Is.Not.Null);
            Assert.That(cell.X, Is.EqualTo(2));
            Assert.That(cell.Y, Is.EqualTo(3));
            Assert.That(cell.State, Is.EqualTo(GridCellState.Empty));
            Assert.That(cell.OccupiedById, Is.EqualTo(0));
            Assert.That(cell.Parent, Is.EqualTo(plane));
        }

        [Test]
        public void UpdateCellState_ChangeState_ShouldUpdateStateAndOccupier()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var cell = GridCellSystem.CreateGridCell(plane, 0, 0);
            GridCellSystem.UpdateCellState(cell, GridCellState.Occupied, 99);

            Assert.That(cell.State, Is.EqualTo(GridCellState.Occupied));
            Assert.That(cell.OccupiedById, Is.EqualTo(99));
        }

        [Test]
        public void RegisterGridPlane_ShouldAddToComponentDictionary()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var comp = plane.AddComponent<GridPlaneListComponent>(c => c.GridPlanes = new Dictionary<long, GridPlane>());
            GridPlaneListSystem.AddGridPlane(ecsNode, plane);

            Assert.That(comp.GridPlanes.ContainsKey(plane.Id), Is.True);
        }

        [Test]
        public void UnregisterGridPlane_ShouldRemoveFromComponentDictionary()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var comp = plane.AddComponent<GridPlaneListComponent>(c => c.GridPlanes = new Dictionary<long, GridPlane> {{ plane.Id, plane }});
            GridPlaneListSystem.RemoveGridPlane(ecsNode, plane.Id);

            Assert.That(comp.GridPlanes.ContainsKey(plane.Id), Is.False);
        }

        [Test]
        public void GetGridPlane_ValidId_ShouldReturnEntity()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var comp = plane.AddComponent<GridPlaneListComponent>(c => c.GridPlanes = new Dictionary<long, GridPlane> {{ plane.Id, plane }});
            var result = GridPlaneListSystem.GetGridPlane(ecsNode, plane.Id);

            Assert.That(result, Is.EqualTo(plane));
        }

        [Test]
        public void GetCell_InvalidCoordinates_ShouldReturnNull()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var comp = plane.AddComponent<GridCellListComponent>(c => c.Cells = new Dictionary<(int, int), GridCell>());
            var cell = GridCellListSystem.GetCell(plane, 10, 10);

            Assert.That(cell, Is.Null);
        }

        [Test]
        public void GetEmptyCells_ShouldReturnAllEmptyCells()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (0f, 0f));
            var emptyCell = GridCellSystem.CreateGridCell(plane, 0, 0);
            var occCell = GridCellSystem.CreateGridCell(plane, 1, 0);
            GridCellSystem.UpdateCellState(occCell, GridCellState.Occupied, 1);

            var comp = plane.AddComponent<GridCellListComponent>(c => c.Cells = new Dictionary<(int, int), GridCell>
            {
                { (0, 0), emptyCell },
                { (1, 0), occCell }
            });

            var emptyCells = GridCellListSystem.GetEmptyCells(plane).ToList();
            Assert.That(emptyCells.Count, Is.EqualTo(1));
            Assert.That(emptyCells[0].State, Is.EqualTo(GridCellState.Empty));
        }

        [Test]
        public void GridToWorld_ShouldReturnCorrectCoordinates()
        {
            var position = (4f, 5f);
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, position);
            var comp = plane.AddComponent<GridConverterComponent>(c => { c.Position = position; c.CellSize = 2f; });

            var world = GridConverterSystem.GridToWorld(plane, 3, 4);
            Assert.That(world.x, Is.EqualTo(position.Item1 + 3 * comp.CellSize));
            Assert.That(world.y, Is.EqualTo(position.Item2 + 4 * comp.CellSize));
        }

        [Test]
        public void InitializeGrid_ValidConfig_ShouldInitializeGridProperties()
        {
            var plane = GridPlaneSystem.CreateGridPlane(ecsNode, config, (1f, 1f));

            Assert.That(plane.Width, Is.EqualTo(config.Width));
            Assert.That(plane.Height, Is.EqualTo(config.Height));
            Assert.That(plane.CellSize, Is.EqualTo(config.CellSize));
        }
    }
}
