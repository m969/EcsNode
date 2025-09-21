# （单位派遣）模块需求文档

## 模块概述

- 目标：在 Unity/ECS 环境下，将“单位”（Entity）按策略派遣，保证简单、稳定、可扩展。
- 适用场景：RTS、塔防、模拟经营等需要把多单位分配到点位、资源、敌人或岗位的系统。
- 范畴边界：仅负责“选择谁去哪里”的调度与再调度；不包含寻路、战斗、动画等，由外部系统通过事件/接口解耦集成。

- 版本 1.0.2 更新摘要：
	- 增加 `DispatchComponent` 与 `DispatchSystem`，用于管理派遣执行体（execution）及相关调度逻辑。
	- 将原有的 `DispatchUnitComponent` 重构为 `DispatchExecution`（作为派遣执行实体），并将 `UnitDispatchSystem` 重命名/重构为 `DispatchExecutionSystem`。
	- 新增 `DispatchExecutionListComponent` 与 `DispatchExecutionListSystem`，用于在宿主实体上管理多个派遣执行体的集合与生命周期。

- 版本 1.0.3 更新摘要：
	- 将 `DispatchExecution` 实体重命名为 `UnitDispatcher`。
	- 将 `DispatchExecutionSystem` 重命名为 `UnitDispatcherSystem`。
	- 将 `DispatchExecutionListComponent` 重命名为 `UnitDispatcherListComponent`。
	- 将 `DispatchExecutionListSystem` 重命名为 `UnitDispatcherListSystem`。

## 模块功能（基础功能列表）

- 单位注册
	- 注册/注销单位

- 分配与再分配
	- 支持即时分配与批处理分配（逐帧/定时），可配置频率

- 结果输出与查询
	- 事件：OnAssigned/OnUnassigned

## 其他依赖模块
- 无