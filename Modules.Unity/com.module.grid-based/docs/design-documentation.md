# （网格系统）模块功能设计文档
## 1. 模块功能概述
网格系统模块用于实现游戏中的格子化建造玩法，负责网格区域的初始化、格子状态管理。模块还提供格子坐标与世界坐标的转换接口。核心功能包括：
- 网格区域的创建和初始化
- 格子状态的查询和更新
- 坐标系统的转换

## 2. 配置接口设计
- IGridPlaneConfig：网格区域配置接口
  - Id：网格区域配置唯一标识
  - Key：网格区域配置名称
  - Width：网格区域宽度（格子数量）
  - Height：网格区域高度（格子数量）
  - CellSize：单个格子的大小（世界单位）
  - InitialState：格子初始状态

## 3. 实体功能设计
### 实体属性设计
- GridPlane（继承自EcsEntity）：表示网格区域实体
  - ConfigId：网格配置ID
  - Width：网格宽度
  - Height：网格高度
  - CellSize：格子大小
  - Position：网格区域世界坐标位置

- GridCell（继承自EcsEntity）：表示单个格子实体
  - GridPlaneId：所属网格区域ID
  - X：格子X坐标（网格坐标系）
  - Y：格子Y坐标（网格坐标系）
  - State：格子状态（空置/占用）
  - OccupiedById：占用者实体ID

### 系统设计
- GridPlaneSystem：网格区域系统，负责：
  - 创建和初始化网格区域
  - 管理网格区域的生命周期
  - 提供网格区域查询接口

- GridCellSystem：格子系统，负责：
  - 创建和初始化格子实体
  - 管理格子状态的变更
  - 提供格子状态查询接口

## 4. 组件功能设计
### 组件属性设计
- GridPlaneListComponent（继承自EcsComponent）：网格区域管理组件
  - GridPlanes：网格区域实体字典 <long, GridPlane>

- GridCellListComponent（继承自EcsComponent）：格子管理组件
  - Cells：格子实体字典 <(int x, int y), GridCell>

- GridConverterComponent（继承自EcsComponent）：坐标转换组件
  - GridPlaneId：关联的网格区域ID
  - Position：网格区域世界坐标位置
  - CellSize：格子大小

### 系统设计
- GridPlaneListSystem：网格区域管理系统，负责：
  - 注册和注销网格区域
  - 提供网格区域查询接口

- GridCellListSystem：格子管理系统，负责：
  - 注册和注销格子实体
  - 提供格子查询和状态更新接口

- GridConverterSystem：坐标转换系统，负责：
  - 网格坐标转世界坐标
  - 世界坐标转网格坐标
  - 网格区域边界检查