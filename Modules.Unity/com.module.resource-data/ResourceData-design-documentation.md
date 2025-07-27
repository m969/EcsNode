# 玩家资源模块功能设计文档

## 1. 模块功能概述

玩家资源模块负责管理玩家在游戏中的各类资源，包括金币、钻石、体力、道具等。模块支持资源的获取、消耗、变更、同步等操作，确保资源数据的准确性和安全性，并与玩家、道具、商城等其他模块进行交互。

## 2. 模块相关实体功能设计

### 资源变更记录实体（ResourceChangeLog）

- **属性设计**
    - `ChangeTime`：变更时间（DateTime）
    - `ChangeType`：变更类型（枚举，如获取、消耗等）
    - `ChangeValue`：变更数值（int）
    - `Reason`：变更原因（string）
    - `ResourceType`：资源类型（枚举）
    - `OwnerId`：所属玩家Id（long）

- **系统功能设计**
    - 记录资源变更历史，支持查询和统计
    - 生命周期接口：Awake、Init、AfterInit、Enable、Disable、Update、Destroy

### 资源变更记录实体系统（ResourceChangeLogSystem）

- **系统功能设计**
    - 资源变更记录实体初始化
    - 记录资源变更历史（如每次资源变更生成日志）
    - 查询资源变更记录（如按时间、类型、玩家等条件检索）
    - 统计资源变更数据（如总获取、总消耗等）
    - 资源变更记录生命周期管理（Awake、Init、AfterInit、Enable、Disable、Update、Destroy）
    - 归档与持久化资源变更记录
    - 触发资源变更日志相关事件（如数据分析、成就统计等）

## 3. 模块相关组件功能设计

### 资源数据组件（ResourceDataComponent）

- **属性设计**
    - `ResourceValues`：资源类型到数值的映射（Dictionary<ResourceType, int>）
    - `LastSyncTime`：上次同步时间（DateTime）

- **系统功能设计**
    - 资源数据的同步、批量变更、校验等操作

### 资源变更组件（ResourceChangeComponent）

- **属性设计**
    - `ChangeType`：变更类型（枚举）
    - `ChangeValue`：变更数值（int）
    - `Reason`：变更原因（string）
    - `ResourceType`：资源类型（枚举）
    - `ChangeTime`：变更时间（DateTime）

- **系统功能设计**
    - 处理单次资源变更，生成变更记录，触发相关事件

### 资源数据系统（ResourceDataSystem）

- **系统功能设计**
    - 资源实体初始化与同步
    - 资源获取（如增加金币、钻石等）
    - 资源消耗（如减少体力、金币等）
    - 资源变更（如奖励、扣除等多种类型）
    - 校验资源合法性（如是否足够、是否越权）
    - 查询资源实体属性
    - 资源实体生命周期管理（Awake、Init、AfterInit、Enable、Disable、Update、Destroy）
    - 触发资源相关事件（如资源变更通知、UI刷新等）
    - 资源数据初始化与同步
    - 资源批量变更（如批量增加/减少多种资源）
    - 资源数值校验（如检测资源是否足够）
    - 资源数据重置（如每日重置体力等）
    - 资源数据持久化（如保存到数据库或本地）
    - 查询指定资源类型的数值
    - 触发资源变更事件

### 资源变更系统（ResourceChangeSystem）

- **系统功能设计**
    - 处理单次资源变更（如单次金币消耗或获取）
    - 生成并记录资源变更日志
    - 校验资源变更合法性（如防止负数、越权等）
    - 触发资源变更相关事件（如UI刷新、成就统计等）
    - 资源变更原因归类与统计
    - 支持多种变更类型（如获取、消耗、奖励、扣除等）
    - 与资源实体和资源数据组件联动，自动同步资源数值

## 4. 系统功能设计

- 资源实体系统（ResourceSystem）：负责资源的初始化、同步、变更、获取、消耗等逻辑
- 资源变更记录实体系统（ResourceChangeLogSystem）：负责记录和管理资源变更历史
- 资源数据系统（ResourceDataSystem）：负责资源数据的批量操作和同步
- 资源变更系统（ResourceChangeSystem）：负责处理单次资源变更逻辑

各系统均实现EcsNode标准生命周期接口（Awake、Init、AfterInit、Enable、Disable、Update、Destroy），并根据实际业务补充静态业务逻辑接口方法。
