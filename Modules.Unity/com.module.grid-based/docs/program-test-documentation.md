# （网格系统）模块程序单元测试设计文档 - version：1.0.3

## 1. 测试概述
- 测试目标：验证网格系统模块中各接口的正确性及边界条件处理。
- 测试范围：
  - 实体系统：GridPlaneSystem、GridCellSystem
  - 组件系统：GridPlaneListSystem、GridCellListSystem、GridConverterSystem
  - 坐标转换功能：GridToWorld

## 2. 测试用例设计
### 2.1 实体系统
- 用例 2.1.1：CreateGridPlane_ValidParameters_ShouldCreateGridPlaneEntity
  - 前置条件：提供有效父实体、IGridPlaneConfig实例、位置参数
  - 测试步骤：调用GridPlaneSystem.CreateGridPlane
  - 预期结果：返回的GridPlane实体不为null，ConfigId、Width、Height、CellSize、Position与配置和参数一致，实体已添加到父实体下

- 用例 2.1.2：CreateGridPlane_InvalidConfigId_ShouldThrowException
  - 前置条件：配置接口中Id不存在或负值
  - 测试步骤：调用GridPlaneSystem.CreateGridPlane
  - 预期结果：抛出参数异常或配置不存在异常

- 用例 2.1.3：CreateGridCell_ValidParameters_ShouldCreateGridCellEntity
  - 前置条件：已有GridPlane实体
  - 测试步骤：调用GridCellSystem.CreateGridCell
  - 预期结果：返回的GridCell实体不为null，X、Y与参数一致，State为默认Empty，OccupierId为0，并添加到GridPlane下

- 用例 2.1.4：UpdateCellState_ChangeState_ShouldUpdateStateAndOccupier
  - 前置条件：已有GridCell实体
  - 测试步骤：调用GridCellSystem.UpdateCellState("Occupied",指定ID)
  - 预期结果：State更新为Occupied，OccupiedById与输入一致

### 2.2 组件系统
- 用例 2.2.1：RegisterGridPlane_ShouldAddToComponentDictionary
  - 测试步骤：对GridPlane实体调用RegisterGridPlane
  - 预期结果：GridPlaneListComponent中包含该实体

- 用例 2.2.2：UnregisterGridPlane_ShouldRemoveFromComponentDictionary
  - 测试步骤：对已注册实体调用UnregisterGridPlane
  - 预期结果：GridPlaneListComponent不包含该实体

- 用例 2.2.3：GetGridPlane_ValidId_ShouldReturnEntity
  - 测试步骤：调用GetGridPlane并传入已注册ID
  - 预期结果：返回对应GridPlane实体

- 用例 2.2.4：GetCell_InvalidCoordinates_ShouldReturnNull
  - 测试步骤：调用GridCellListSystem.GetCell并传入不存在的坐标
  - 预期结果：返回null或默认值

- 用例 2.2.5：GetEmptyCells_ShouldReturnAllEmptyCells
  - 测试步骤：为GridPlane设置混合状态的若干GridCell
  - 预期结果：返回列表中仅包含State为Empty的GridCell

### 2.3 转换系统
- 用例 2.3.1：GridToWorld_ShouldReturnCorrectCoordinates
  - 前置条件：已配置GridConverterComponent
  - 测试步骤：调用GridConverterSystem.GridToWorld
  - 预期结果：返回值与(cellIndex * CellSize + Position)一致

## 3. 测试执行计划
- 测试框架：使用Unity Test Runner + NUnit
- 环境准备：Mock IGridPlaneConfig实现类，初始化必要实体和组件
- 执行顺序：先实体系统，再组件系统，最后坐标转换测试
- 自动化脚本：编写TestFixture类组织测试用例

## 4. 测试结果记录
- 使用表格记录关键字段：
  | 用例编号 | 测试项 | 输入 | 预期结果 | 实际结果 | 备注 |
  | -------- | ------ | ---- | -------- | -------- | ---- |
  | 2.1.1    | CreateGridPlane | 有效参数 | 实体创建成功 | 通过 |      |
  | 2.1.2    | CreateGridPlane | 无效配置 | 抛出异常 | 通过 |      |
  | ...      | ...    | ...  | ...      | ...      | ...  |
