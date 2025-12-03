# Resource Data System API Documentation

## 命名空间
`ECSGame.ResourceDataModule`

## 系统 (Systems)

### ResourceDataSystem
资源数据管理系统，继承自 `AComponentSystem<EcsEntity, ResourceDataComponent>`。
负责资源的增删改查、合法性校验、日志记录和事件分发。

#### 静态方法

##### GainResource
增加资源：内部执行合法性校验、日志写入、同步。

```csharp
public static void GainResource(EcsEntity entity, ResourceType type, int value, long itemId = 0)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |
| type | ResourceType | 资源类型 |
| value | int | 增加数值 |
| itemId | long | 关联物品ID（可选，默认为0） |

##### ConsumeResource
消耗资源：不足返回false；成功则写日志与同步。

```csharp
public static bool ConsumeResource(EcsEntity entity, ResourceType type, int value, long itemId = 0)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |
| type | ResourceType | 资源类型 |
| value | int | 消耗数值 |
| itemId | long | 关联物品ID（可选，默认为0） |
| **返回** | bool | 是否消耗成功 |

##### SyncResource
同步资源数据。更新 `LastSyncTime` 并派发 `IResourceSynced` 事件。

```csharp
public static void SyncResource(EcsEntity entity)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |

##### ValidateResource
校验资源合法性。检查资源是否足够。

```csharp
public static bool ValidateResource(EcsEntity entity, ResourceType type, int value)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |
| type | ResourceType | 资源类型 |
| value | int | 需要校验的数值 |
| **返回** | bool | 资源是否足够 |

##### GetResourceValue
查询资源数值。

```csharp
public static int GetResourceValue(EcsEntity entity, ResourceType type)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |
| type | ResourceType | 资源类型 |
| **返回** | int | 当前资源数值 |

##### BatchChange
批量变更资源：原子预检，任一变更导致负值则整体取消。变更成功后逐条写日志与同步。

```csharp
public static bool BatchChange(EcsEntity entity, Dictionary<ResourceType, int> changes)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 目标实体 |
| changes | Dictionary<ResourceType, int> | 变更字典（Key: 资源类型, Value: 变更值，正增负减） |
| **返回** | bool | 是否批量变更成功 |

### ResourceChangeLogSystem
资源变更日志系统，继承自 `AEntitySystem<ResourceChangeLog>`。
负责资源变更日志的创建、查询和统计。

#### 静态方法

##### AddChangeLog
记录资源变更日志（在owner下创建子实体）。

```csharp
public static ResourceChangeLog AddChangeLog(EcsEntity owner, ResourceChangeType changeType, int value, string reason, ResourceType type, long ownerId)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| owner | EcsEntity | 日志所属实体（例如玩家或其资源实体） |
| changeType | ResourceChangeType | 变更类型 |
| value | int | 变更数值，正为增加，负为减少 |
| reason | string | 变更原因 |
| type | ResourceType | 资源类型 |
| ownerId | long | 所属玩家Id |
| **返回** | ResourceChangeLog | 创建的日志实体 |

##### QueryChangeLogs
查询资源变更记录。

```csharp
public static List<ResourceChangeLog> QueryChangeLogs(EcsEntity owner, Func<ResourceChangeLog, bool> filter)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| owner | EcsEntity | 日志所属实体 |
| filter | Func<ResourceChangeLog, bool> | 筛选谓词，可为空 |
| **返回** | List<ResourceChangeLog> | 符合条件的日志列表 |

##### StatChangeLogs
统计资源变更数据。

```csharp
public static int StatChangeLogs(EcsEntity owner, ResourceType? type)
```

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| owner | EcsEntity | 日志所属实体 |
| type | ResourceType? | 资源类型（可选，传入null统计所有类型） |
| **返回** | int | 变更数值总和 |
