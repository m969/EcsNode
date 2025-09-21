# 玩家资源模块需求文档
## 1. 模块需求概述

玩家资源模块用于管理玩家在游戏中的各类资源，包括但不限于金币、钻石、体力、道具等。该模块需支持资源的获取、消耗、变更、同步等功能，保证资源数据的准确性和安全性，并能与其他模块（如玩家、道具、商城等）进行交互。

## 2. 模块相关实体功能需求

- 资源变更记录实体（ResourceChangeLog）：用于记录玩家资源的变更历史，便于追溯和统计。

## 3. 模块相关组件功能需求

- 资源数据组件（ResourceDataComponent）：用于存储和管理具体的资源数值，如金币数量、钻石数量等。

备注（1.0.1）：移除资源变更组件（ResourceChangeComponent）。资源变更由资源数据系统（ResourceDataSystem）的接口直接处理，并在需要时写入资源变更记录（ResourceChangeLog）。

## 4. 节点接口派发需求（1.0.2）

为支持外部模块对资源数据变化进行开放式扩展（遵循开闭原则、依赖倒置原则），新增节点接口派发需求：

- 必须提供资源变更派发接口：`IResourceChanged`。
	- 方法签名：`void OnResourceChanged(EcsEntity entity, ResourceType type, int delta, int newValue, ResourceChangeType changeType, string reason);`
	- 触发场景：任何资源数值被增减（单次或批量中的单项）后立即派发。
- 建议提供资源同步完成派发接口：`IResourceSynced`。
	- 方法签名：`void OnResourceSynced(EcsEntity entity, DateTime syncTime);`
	- 触发场景：执行资源同步（如从存储加载或写回后）完成。
- 批量变更不额外定义独立接口，内部逐条派发 `IResourceChanged`，保证无“特殊情况”逻辑分支。
- 所有派发均通过实体的 `Dispatch<TInterface>()` 方法完成，系统静态方法内部不直接依赖实现类，只依赖接口。

版本备注（1.0.2）：上述节点接口为新增需求，历史行为保持兼容，不影响既有逻辑调用方。
