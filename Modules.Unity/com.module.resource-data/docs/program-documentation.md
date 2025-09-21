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
备注（1.0.1）：删除本组件，相关字段在接口参数与变更记录实体中体现。

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

### 节点派发接口（1.0.2 新增）

```csharp
/// <summary>
/// 资源变更事件接口（增/减均触发；批量逐条触发）。
/// </summary>
public interface IResourceChanged : IDispatch
{
    void OnResourceChanged(EcsEntity entity, ResourceType type, int delta, int newValue, ResourceChangeType changeType, string reason);
}

/// <summary>
/// 资源同步完成事件接口（加载或持久化写回完成）。
/// </summary>
public interface IResourceSynced : IDispatch
{
    void OnResourceSynced(EcsEntity entity, DateTime syncTime);
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
    - （1.0.2 补充触发说明）上述所有会导致资源值实际变化的方法在成功写入后调用：
        - `entity.Dispatch<IResourceChanged>(d => d.OnResourceChanged(entity, type, delta, newValue, changeType, reason));`
        - `BatchChange` 内部循环与单次逻辑复用同一派发，无独立批量事件。
        - `SyncResource` / `PersistResource` 完成后调用：`entity.Dispatch<IResourceSynced>(d => d.OnResourceSynced(entity, DateTime.UtcNow));`

#### 资源变更系统（ResourceChangeSystem）
备注（1.0.1）：删除本系统，其职责合并到 ResourceDataSystem：
    - `GainResource`/`ConsumeResource` 内部完成合法性校验、事件触发、变更记录写入。

## 4. 事件派发实现要点（1.0.2）

- 不新增系统状态字段；派发完全基于方法局部变量与组件当前值。
- 批量变更走逐条循环，复用单次变更逻辑，避免“批量特例”分支。
- 新增接口为纯增量，不修改既有方法签名，保持向后兼容。
- 失败（校验不通过）不触发派发；只有实际写入成功后才派发。

版本备注（1.0.2）：新增 `IResourceChanged`、`IResourceSynced` 两个接口以及对应触发策略描述。
