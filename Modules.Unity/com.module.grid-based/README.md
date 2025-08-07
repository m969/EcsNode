# （网格系统）模块使用文档

## 简介
本模块基于 EcsNode 框架，实现了网格区域（GridPlane）和格子（GridCell）的管理与坐标转换功能，方便在 Unity 中构建基于网格的游戏逻辑。

## 特性
- 支持定义多个网格区域（GridPlane），可设置尺寸、位置和配置ID。
- 自动生成格子（GridCell）实体，并管理状态（空置、占用、锁定、选中）。
- 提供世界坐标与网格坐标相互转换（GridToWorld/WorldToGrid）。
- 支持批量管理格子和网格区域的列表组件。
- 事件接口：当网格区域或格子状态变更时，可订阅 `IOnGridPlaneChanged`、`IOnGridCellStateChanged`。

## 安装


## 快速开始
```csharp
using ECS;
using ECSGame.Module.GridBased;

// 假设已有 EcsNode 实例 ecsNode
// 1. 创建网格区域
var plane = GridPlaneSystem.CreateGridPlane(ecsNode, configId: 1001, position: (x: 0f, y: 0f));
// 初始化网格，根据自定义配置组件填充尺寸
GridPlaneSystem.InitializeGrid(plane);

// 2. 添加坐标转换组件
plane.AddComponent<GridConverterComponent>(comp => {
    comp.GridPlaneId = plane.Id;
    comp.Position = plane.Position;
    comp.CellSize = plane.CellSize;
});

// 3. 创建格子并更新状态
var cell = GridCellSystem.CreateGridCell(plane, x: 0, y: 0);
GridCellSystem.UpdateCellState(cell, GridCellState.Occupied, occupierId: 42);

// 4. 坐标转换示例
var worldPos = GridConverterSystem.GridToWorld(plane, gridX: 2, gridY: 3);
var gridPos = GridConverterSystem.WorldToGrid(plane, worldX: worldPos.x, worldY: worldPos.y);
```

## 模块结构
```
com.model.grid-based    // 实体与组件定义
com.system.grid-based   // 系统逻辑实现
system.module-test      // 模块单元测试
docs                    // 设计与使用文档
```

## 测试
测试基于 NUnit，测试类位于 `system.module-test/GridBasedModuleTests.cs`。可运行测试验证核心功能。
