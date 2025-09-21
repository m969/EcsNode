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

备注（1.0.1）：删除资源变更组件（ResourceChangeComponent），由资源数据系统的接口直接接管变更处理并写入变更记录。

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

备注（1.0.1）：删除资源变更系统（ResourceChangeSystem）。相关逻辑合并至资源数据系统（ResourceDataSystem）。

## 4. 系统功能设计

- 资源实体系统（ResourceSystem）：负责资源的初始化、同步、变更、获取、消耗等逻辑
- 资源变更记录实体系统（ResourceChangeLogSystem）：负责记录和管理资源变更历史
- 资源数据系统（ResourceDataSystem）：负责资源数据的批量操作和同步

备注（1.0.1）：删除“资源变更系统（ResourceChangeSystem）”一行，相关职责由“资源数据系统（ResourceDataSystem）”承担。

各系统均实现EcsNode标准生命周期接口（Awake、Init、AfterInit、Enable、Disable、Update、Destroy），并根据实际业务补充静态业务逻辑接口方法。

## 5. 节点接口派发设计（1.0.2）

### 设计目标
通过事件接口（节点派发接口）向外部模块开放资源数据关键生命周期与变更通知，支持解耦的扩展逻辑（统计、成就、UI刷新、任务驱动等），遵循开闭与依赖倒置原则。

### 接口列表

```csharp
public interface IResourceChanged : IDispatch
{
    /// <summary>
    /// 资源变更后回调（增/减均会触发；批量变更逐条触发）。
    /// </summary>
    /// <param name="entity">资源所属实体（玩家实体或其资源子实体）。</param>
    /// <param name="type">资源类型。</param>
    /// <param name="delta">本次变更差值（正为增加，负为消耗）。</param>
    /// <param name="newValue">变更后的最新资源值。</param>
    /// <param name="changeType">业务层标识的变更类型。</param>
    /// <param name="reason">业务来源/原因字符串。</param>
    void OnResourceChanged(EcsEntity entity, ResourceType type, int delta, int newValue, ResourceChangeType changeType, string reason);
}

public interface IResourceSynced : IDispatch
{
    /// <summary>
    /// 资源数据完成一次外部同步（加载或持久化写回）后回调。
    /// </summary>
    /// <param name="entity">资源所属实体。</param>
    /// <param name="syncTime">同步完成时间。</param>
    void OnResourceSynced(EcsEntity entity, DateTime syncTime);
}
```

### 触发策略
- 单次增减：在 `GainResource` / `ConsumeResource` / `BatchChange` 内部每条变更成功应用后派发 `IResourceChanged`。
- 批量变更：不定义新接口，循环内部逐条调用派发（消除批量特殊路径）。
- 同步操作成功：`SyncResource` 或 `PersistResource` 完成后派发 `IResourceSynced`。

### 设计要点
- 系统方法只依赖接口，不直接引用外部实现，保持模块内零耦合。
- 批量与单次复用同一派发路径，避免条件分支爆炸。
- 保持既有（1.0.1及之前）对外静态方法签名不变，新增行为为“增量式”增强。

版本备注（1.0.2）：新增上述节点派发接口与触发策略，无破坏性修改。
