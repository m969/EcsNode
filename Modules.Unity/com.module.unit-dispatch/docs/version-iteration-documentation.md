# （单位派遣）模块版本迭代文档

## 1.0.0 - 初始版本

## 1.0.2 - 功能迭代
- 增加DispatchComponent和DispatchSystem，用于管理派遣执行体及相关逻辑。
- DispatchUnitComponent重构为DispatchExecution实体，DispatchUnitSystem重构为DispatchExecutionSystem。
- 增加DispatchExecutionListComponent和DispatchExecutionListSystem，用于管理多个DispatchExecution实体。

## 1.0.3 - 功能迭代
- DispatchExecution实体重命名为UnitDispatcher
- DispatchExecutionSystem重命名为UnitDispatcherSystem
- DispatchExecutionListComponent实体重命名为UnitDispatcherListComponent
- DispatchExecutionListSystem重命名为UnitDispatcherListSystem