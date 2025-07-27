# 玩家资源模块程序设计文档

## 1. 模块程序功能概述

玩家资源模块用于管理玩家的各类资源（如金币、钻石、体力、道具等），支持资源的获取、消耗、变更、同步、统计和日志记录，确保资源数据的准确性和安全性，并与其他模块（如玩家、道具、商城等）进行交互。

## 2. 模块程序数据结构

### 实体（Entities）设计

#### 资源变更记录实体（ResourceChangeLog）
- `ChangeTime`：变更时间（DateTime）
- `ChangeType`：变更类型（枚举，如获取、消耗等）
- `ChangeValue`：变更数值（int）
- `Reason`：变更原因（string）
- `ResourceType`：资源类型（枚举）
- `OwnerId`：所属玩家Id（long）

### 组件（Components）设计

#### 资源数据组件（ResourceDataComponent）
- `ResourceValues`：资源类型到数值的映射（Dictionary<ResourceType, int>）
- `LastSyncTime`：上次同步时间（DateTime）

#### 资源变更组件（ResourceChangeComponent）
- `ChangeType`：变更类型（枚举）
- `ChangeValue`：变更数值（int）
- `Reason`：变更原因（string）
- `ResourceType`：资源类型（枚举）
- `ChangeTime`：变更时间（DateTime）

### 类型补充

#### 枚举类型

```csharp
/// <summary>
/// 资源类型枚举
/// </summary>
public enum ResourceType
{
    Coin,
    Diamond,
    Stamina,
    Item,
    // 可扩展更多类型
}

/// <summary>
/// 资源变更类型枚举
/// </summary>
public enum ResourceChangeType
{
    Gain,
    Consume,
    Reward,
    Deduct,
    // 可扩展更多类型
}
```

## 3. 模块程序逻辑流程

### 系统（Systems）设计

#### 资源变更记录实体系统（ResourceChangeLogSystem）
- 生命周期接口：Awake、Init、AfterInit、Enable、Disable、Update、Destroy
- 静态接口方法：
    - `AddChangeLog(entity, changeType, value, reason, type, ownerId)`：记录资源变更日志
    - `QueryChangeLogs(entity, filter)`：查询资源变更记录
    - `StatChangeLogs(entity, type)`：统计资源变更数据

#### 资源数据系统（ResourceDataSystem）
- 生命周期接口：Awake、Init、AfterInit、Enable、Disable、Update、Destroy
- 静态接口方法：
    - `GainResource(entity, type, value, itemId)`：增加资源
    - `ConsumeResource(entity, type, value, itemId)`：消耗资源
    - `SyncResource(entity)`：同步资源数据
    - `ValidateResource(entity, type, value)`：校验资源合法性
    - `GetResourceValue(entity, type)`：查询资源数值
    - `BatchChange(entity, changes)`：批量变更资源
    - `ResetResource(entity)`：重置资源数据
    - `PersistResource(entity)`：持久化资源数据

#### 资源变更系统（ResourceChangeSystem）
- 生命周期接口：Awake、Init、AfterInit、Enable、Disable、Update、Destroy
- 静态接口方法：
    - `HandleChange(entity, changeType, value, reason, type)`：处理单次资源变更
    - `ValidateChange(entity, changeType, value, type)`：校验资源变更合法性
    - `TriggerChangeEvent(entity, changeType, value, type)`：触发资源变更相关事件

## 4. 模块程序测试方案

- 资源获取/消耗/同步/变更的单元测试
- 资源变更日志记录与查询测试
- 资源批量变更与重置测试
- 资源变更合法性校验测试
- 资源与道具物品结合场景测试
- 边界条件与异常处理测试
